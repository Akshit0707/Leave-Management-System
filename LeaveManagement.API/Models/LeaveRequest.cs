using System.Reflection.Metadata;

namespace LeaveManagement.API.Models
{
    public class LeaveRequest
    {
        public int Id{get; set;}
        public int UserId{get; set;}
        public User User{get; set;}= null!;
        public DateTime StartDate{get; set;}
        public DateTime EndDate{get; set;}
        public string Reason{get; set;}= string.Empty;
        public LeaveRequestStatus Status{get; set;}= LeaveRequestStatus.Pending;
        public string? ManagerComments{get; set;}
        public int? ReviewedById{get; set;}
        public DateTime? ReviewedAt{get; set;}
        public DateTime CreatedAt{get; set;}= DateTime.UtcNow;
        public DateTime UpdatedAt{get; set;}= DateTime.UtcNow;
    }
    public enum LeaveRequestStatus
    {//0,1,2 means that in database it these status will be stored as integers rather than strings to avoid spelling mistakes or any confusion.
        Pending=0,
        Approved=1,
        Rejected=2
    }
}