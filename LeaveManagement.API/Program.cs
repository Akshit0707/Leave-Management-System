using System.Text;
using LeaveManagement.API.Data;
using LeaveManagement.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LeaveManagement.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        /* =======================
           CONTROLLERS & JSON
        ======================= */
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                );
            });

        /* =======================
           CORS (NETLIFY + LOCAL)
        ======================= */
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins(
                        "https://symphonious-biscotti-b6fc95.netlify.app", // 🔴 REPLACE with your Netlify URL
                        "http://localhost:4200"               // Angular local dev
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                // ❌ DO NOT use AllowCredentials() for JWT
            });
        });

        /* =======================
           DATABASE (AZURE SQL)
        ======================= */
        var connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Database connection string not found.");

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure();
            });
        });

        /* =======================
           DEPENDENCY INJECTION
        ======================= */
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ILeaveService, LeaveService>();

        /* =======================
           JWT AUTHENTICATION
        ======================= */
        var jwtKey = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new InvalidOperationException("JWT Key missing.");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
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

        /* =======================
           SWAGGER
        ======================= */
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
           MIDDLEWARE PIPELINE
        ======================= */
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseCors("AllowFrontend");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapGet("/health", () => Results.Ok("Healthy"));

        app.Run();
    }
}
