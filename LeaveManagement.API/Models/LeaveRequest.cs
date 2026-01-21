using System.Reflection.Metadata;

namespace LeaveManagement.API.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;
        public string? ManagerComments { get; set; }
        public int? ReviewedById { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public DateTime CreatedAt { get; set; }  // Removed default
        public DateTime UpdatedAt { get; set; }  // Removed default
    }
    
    public enum LeaveRequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}