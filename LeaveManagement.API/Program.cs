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
           CORS
        ======================= */
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAzureFrontend", policy =>
            {
                policy.WithOrigins(
                    "https://purple-forest-05b8b3100.4.azurestaticapps.net",
                    "http://localhost:4200"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            });
        });

        /* =======================
           DATABASE (Switched to SQL Server)
        ======================= */
        // Azure App Service automatically injects connection strings 
        // as environment variables prefixed with SQLAZURECONNSTR_
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                               ?? Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection");

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions => 
            {
                // Recommended for Azure SQL to handle transient connection drops
                sqlOptions.EnableRetryOnFailure(); 
            }));

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ILeaveService, LeaveService>();

        /* =======================
           JWT AUTH
        ======================= */
        var jwtKey = builder.Configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key missing from configuration");

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
            // This will now use the SQL Server provider
            db.Database.Migrate();
        }

        /* =======================
           PIPELINE
        ======================= */
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();
        app.UseCors("AllowAzureFrontend");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapGet("/health", () => Results.Ok("Healthy"));

        app.Run();
    }
}