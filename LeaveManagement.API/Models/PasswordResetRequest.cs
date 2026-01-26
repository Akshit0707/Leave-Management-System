using System;
using System.ComponentModel.DataAnnotations;
using LeaveManagement.API.Models;

namespace LeaveManagement.API.Models;

public class PasswordResetRequest
{
    public int Id { get; set; }
    [Required]
    public string Email { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public bool IsApproved { get; set; } = false;
    public bool IsCompleted { get; set; } = false;
    public bool IsRejected { get; set; } = false;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // New column for storing the new password during reset
    public string? NewPassword { get; set; }

    // New column for storing admin comment on rejection/approval
    public string? Comment { get; set; }
}
