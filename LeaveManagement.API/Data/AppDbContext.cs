using LeaveManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).IsRequired().HasMaxLength(100);
            e.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            e.Property(x => x.LastName).IsRequired().HasMaxLength(50);
            
            // Ensure DateTime columns are UTC
            e.Property(x => x.CreatedAt)
                .HasConversion(v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc));

            e.HasOne(x => x.Manager)
             .WithMany()
             .HasForeignKey(x => x.ManagerId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // LeaveRequest entity
        modelBuilder.Entity<LeaveRequest>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Reason).IsRequired().HasMaxLength(500);
            
            // Ensure all DateTime columns are UTC
            e.Property(x => x.StartDate)
                .HasConversion(v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc));
            e.Property(x => x.EndDate)
                .HasConversion(v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc));
            e.Property(x => x.CreatedAt)
                .HasConversion(v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc));
            e.Property(x => x.UpdatedAt)
                .HasConversion(v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc));
            e.Property(x => x.ReviewedAt)
                .HasConversion(v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)) : null);

            e.HasOne(x => x.User)
             .WithMany(u => u.LeaveRequests)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override int SaveChanges()
    {
        NormalizeDateTimes();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        NormalizeDateTimes();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void NormalizeDateTimes()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            foreach (var property in entry.Properties)
            {
                if (property.CurrentValue is DateTime dt && dt.Kind == DateTimeKind.Unspecified)
                {
                    property.CurrentValue = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                }
            }
        }
    }
}