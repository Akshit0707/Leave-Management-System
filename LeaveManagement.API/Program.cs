using System.Text;
using LeaveManagement.API.Data;
using LeaveManagement.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
                // 🔥 FIX: Accept enum values safely (Manager / manager / MANAGER)
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter()
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
                        "https://symphonious-biscotti-b6fc95.netlify.app",
                        "http://localhost:4200"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                // ✅ JWT → no AllowCredentials()
            });
        });

        /* =======================
           DATABASE (AZURE SQL)
        ======================= */
        var connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("Azure SQL connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure();
            })
        );

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
            throw new Exception("JWT Key missing. Set Jwt__Key in Azure.");

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
