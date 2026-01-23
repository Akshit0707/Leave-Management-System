using System.Text;
using LeaveManagement.API.Data;
using LeaveManagement.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// PostgreSQL timestamp fix
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

/* =======================
   RAILWAY PORT CONFIG
======================= */
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

/* =======================
   CONFIGURATION
======================= */
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables();

/* =======================
   CORS (STRICT & CORRECT)
======================= */
var allowedOrigins = builder.Configuration["AllowedOrigins"]
    ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? throw new InvalidOperationException("AllowedOrigins not configured");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

/* =======================
   SERVICES
======================= */
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Leave Management API",
        Version = "v1"
    });
});

/* =======================
   DATABASE (POSTGRESQL)
======================= */
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();

/* =======================
   JWT AUTH
======================= */
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key missing");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

/* =======================
   HTTP PIPELINE (ORDER MATTERS)
======================= */
app.UseSwagger();
app.UseSwaggerUI();

/* 🔴 REQUIRED FOR CORS PREFLIGHT */
app.UseRouting();

/* 🔴 MUST BE AFTER ROUTING */
app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

/* =======================
   HEALTH CHECK
======================= */
app.MapGet("/health", () =>
    Results.Ok(new { status = "healthy", time = DateTime.UtcNow }));

/* =======================
   DATABASE INIT (TEMP)
======================= */
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    Console.WriteLine("Ensuring database exists...");
    db.Database.EnsureCreated(); // replace with Migrate() later
    Console.WriteLine("Database ready");
}
catch (Exception ex)
{
    Console.WriteLine("DATABASE ERROR:");
    Console.WriteLine(ex.Message);
}

Console.WriteLine($"Application started on port {port}");
app.Run();
