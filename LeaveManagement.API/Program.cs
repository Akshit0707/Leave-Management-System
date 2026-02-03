using System.Text;
using LeaveManagement.API.Data;
using LeaveManagement.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace LeaveManagement.API;

public class Program
{
    public static void Main(string[] args)
    {
        // Fix for Postgres timestamp issues
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var builder = WebApplication.CreateBuilder(args);

        /* =======================
           SERVICES & JSON
        ======================= */
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                );
            });

        /* =======================
           CORS (Updated for Azure)
        ======================= */
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAzureFrontend", policy =>
            {
                policy.WithOrigins(
                    "https://purple-forest-05b8b3100.4.azurestaticapps.net", // Your Static Web App
                    "http://localhost:4200"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // Required if using HttpOnly cookies or specific Auth headers
            });
        });

        /* =======================
           DATABASE
        ======================= */
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(GetConnectionString(builder)));

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ILeaveService, LeaveService>();

        /* =======================
           JWT AUTH
        ======================= */
        var jwtKey = builder.Configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key missing");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    ),
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        /* =======================
           APPLY MIGRATIONS
        ======================= */
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        /* =======================
           PIPELINE (Order is Crucial)
        ======================= */
        // Always show Swagger in this project phase for debugging
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        // Must be between UseRouting and UseAuthentication
        app.UseCors("AllowAzureFrontend");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        
        // Health check endpoint to verify routing is working
        app.MapGet("/health", () => Results.Ok("Healthy"));

        app.Run();
    }

    private static string GetConnectionString(WebApplicationBuilder builder)
    {
        // Check for Azure App Service Environment Variable
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        
        if (string.IsNullOrEmpty(databaseUrl))
            return builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

        if (databaseUrl.Contains("Host="))
            return databaseUrl;

        // Parse Railway-style URL if still using that format
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        var csBuilder = new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = uri.AbsolutePath.TrimStart('/'),
            SslMode = Npgsql.SslMode.Prefer,
            TrustServerCertificate = true // Often needed for cloud DBs
        };
        return csBuilder.ToString();
    }
}