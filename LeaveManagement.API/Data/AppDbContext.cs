using LeaveManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LeaveManagement.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<PasswordResetRequest> PasswordResetRequests => Set<PasswordResetRequest>();

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

            e.HasOne(x => x.User)
             .WithMany(u => u.LeaveRequests)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // PasswordResetRequest entity
        modelBuilder.Entity<PasswordResetRequest>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Email).IsRequired().HasMaxLength(100);
            e.Property(x => x.RequestedAt).IsRequired();
        });
    }

    public override int SaveChanges()
    {
        ConvertDatesToUtc();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ConvertDatesToUtc();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ConvertDatesToUtc()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            foreach (var property in entry.Properties)
            {
                if (property.Metadata.ClrType == typeof(DateTime))
                {
                    if (property.CurrentValue is DateTime dateTime && dateTime.Kind != DateTimeKind.Utc)
                    {
                        property.CurrentValue = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                    }
                }
                else if (property.Metadata.ClrType == typeof(DateTime?))
                {
                    if (property.CurrentValue is DateTime dateTimeNullable && dateTimeNullable.Kind != DateTimeKind.Utc)
                    {
                        property.CurrentValue = DateTime.SpecifyKind(dateTimeNullable, DateTimeKind.Utc);
                    }
                }
            }
        }
    }
}