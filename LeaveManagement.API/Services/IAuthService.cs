using LeaveManagement.API.DTOs;
using LeaveManagement.API.Models;

namespace LeaveManagement.API.Services;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<bool> RequestPasswordResetAsync(string email);
    Task<List<PasswordResetRequest>> GetPendingPasswordResetRequestsAsync();
    Task<bool> ApprovePasswordResetAsync(int requestId);
    Task<bool> CompletePasswordResetAsync(int requestId, string newPassword);
}