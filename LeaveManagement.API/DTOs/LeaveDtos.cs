using LeaveManagement.API.Models;
namespace LeaveManagement.API.DTOs;
public class CreateLeaveRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
}
public class UpdateLeaveRequest
{
    public LeaveRequestStatus Status { get; set; }
    public string? ManagerComments { get; set; }
}
public class LeaveRequestDto
{
    public int Id{get; set;}
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysCount { get; set; }  
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ManagerComments { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
}
public class LeaveSummaryDto
{
    public int TotalRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public int PendingRequests { get; set; }
    public int TotalDaysRequested { get; set; }
    public int TotalDaysApproved { get; set; }
}