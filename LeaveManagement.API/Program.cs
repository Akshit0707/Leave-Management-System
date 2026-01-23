using System.Text;
using LeaveManagement.API.Data;
using LeaveManagement.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text.Json;


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        );
    });


/* =======================
   RAILWAY PORT
======================= */
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(int.Parse(port));
});

/* =======================
   CONFIG
======================= */
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables();

/* =======================
   CORS
======================= */
var allowedOrigins = builder.Configuration["AllowedOrigins"]
    ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? new[]
    {
        "http://localhost:4200",
        "https://leavemgmtsy.netlify.app"
    };

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
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* =======================
   DATABASE (RAILWAY SAFE)
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

var app = builder.Build();

/* =======================
   APPLY MIGRATIONS
======================= */
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}




/* =======================
   PIPELINE
======================= */
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();

/* =======================
   HELPER METHODS
======================= */
string GetConnectionString(WebApplicationBuilder builder)
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (string.IsNullOrEmpty(databaseUrl))
#pragma warning disable CS8603 // Possible null reference return.
        return builder.Configuration.GetConnectionString("DefaultConnection");
#pragma warning restore CS8603 // Possible null reference return.

    // If it's already in ADO.NET format, just return it
    if (databaseUrl.Contains("Host="))
        return databaseUrl;

    // Otherwise, parse the URL format
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var csBuilder = new Npgsql.NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port,
        Username = userInfo[0],
        Password = userInfo[1],
        Database = uri.AbsolutePath.TrimStart('/'),
        SslMode = Npgsql.SslMode.Prefer
    };
    return csBuilder.ToString();
}
