using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using LeaveManagement.API.Data;
using LeaveManagement.API.DTOs;
using LeaveManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LeaveManagement.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request body cannot be null.");
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required.");
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password is required.");
            if (string.IsNullOrWhiteSpace(request.FirstName))
                throw new ArgumentException("First name is required.");
            if (string.IsNullOrWhiteSpace(request.LastName))
                throw new ArgumentException("Last name is required.");
            // Validate email doesn't exist
            if (await _db.Users.AnyAsync(u => u.Email == request.Email))
                return null;

            // Use UserRole from request
            UserRole userRole = request.Role;

            // Validate ManagerId if user is Employee
            if (request.ManagerId.HasValue && userRole == UserRole.Employee)
            {
                var managerExists = await _db.Users.AnyAsync(u => u.Id == request.ManagerId.Value && u.Role == UserRole.Manager);
                if (!managerExists)
                    throw new ArgumentException("Invalid Manager ID");
            }

            var user = new User
            {
                Email = request.Email ?? string.Empty,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password ?? string.Empty),
                FirstName = request.FirstName ?? string.Empty,
                LastName = request.LastName ?? string.Empty,
                Role = userRole,
                ManagerId = userRole == UserRole.Manager ? null : request.ManagerId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return BuildToken(user);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RegisterAsync error: {ex.Message}");
            if (ex.InnerException != null) Console.WriteLine($"Inner: {ex.InnerException.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) return null;

        return BuildToken(user);
    }

    private AuthResponse BuildToken(User user)
    {
        var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key missing");
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];
        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("name", $"{user.FirstName} {user.LastName}")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiresMinutes"] ?? "60")),
            signingCredentials: creds);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            Token = new JwtSecurityTokenHandler().WriteToken(token)
        };
    }
}