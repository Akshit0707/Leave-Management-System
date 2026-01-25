namespace LeaveManagement.API.DTOs;
public class CompletePasswordResetDto
{
    public int RequestId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}