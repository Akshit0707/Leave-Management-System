using LeaveManagement.API.Models;
namespace LeaveManagement.API.DTOs;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string ?FirstName { get; set; }
    public string ?LastName { get; set; }
    public string ?Email { get; set; }
    public string ?Password { get; set; }
    public UserRole Role { get; set; }   // 👈 make it an enum
    public int? ManagerId { get; set; }
}

public class AuthResponse
{
    public int UserId { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Role { get; set; } = string.Empty; // Always string for frontend
    public string Token { get; set; } = string.Empty;
}
