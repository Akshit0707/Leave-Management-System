namespace LeaveManagement.API.Models;

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;  // ✅ Fixed
        public string PasswordHash { get; set; }= string.Empty;//We use password hash so that user password gets encrypted and safe from threats.
        public string FirstName { get; set; }= string.Empty;
        public string LastName { get; set; }= string.Empty;

        public UserRole Role { get; set; }
        public int? ManagerId { get; set; }
        public User? Manager { get; set; }

        public ICollection<LeaveRequest> LeaveRequests{get; set;}= new List<LeaveRequest>();
        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;

    }
    public enum UserRole
    {
      Employee=0,
      Manager=1
    }

