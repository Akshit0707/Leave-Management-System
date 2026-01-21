using LeaveManagement.API.Data;
using LeaveManagement.API.DTOs;
using LeaveManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Services;

public class LeaveService : ILeaveService
{
    private readonly AppDbContext _db;

    public LeaveService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<LeaveRequestDto?> CreateAsync(int userId, CreateLeaveRequest request)
    {
        if (request.StartDate.Date > request.EndDate.Date)
            return null;

        var entity = new LeaveRequest
        {
            UserId = userId,
            StartDate = request.StartDate.Date,
            EndDate = request.EndDate.Date,
            Reason = request.Reason,
            Status = LeaveRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.LeaveRequests.Add(entity);
        await _db.SaveChangesAsync();
        await _db.Entry(entity).Reference(e => e.User).LoadAsync();

        return MapToDto(entity);
    }

    public async Task<IEnumerable<LeaveRequestDto>> GetMyAsync(int userId)
    {
        var items = await _db.LeaveRequests
            .Include(l => l.User)
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<LeaveRequestDto>> GetPendingAsync()
    {
        var items = await _db.LeaveRequests
            .Include(l => l.User)
            .Where(l => l.Status == LeaveRequestStatus.Pending)
            .OrderBy(l => l.CreatedAt)
            .ToListAsync();

        return items.Select(MapToDto);
    }

    public async Task<bool> UpdateStatusAsync(int id, UpdateLeaveRequest request, int managerId)
    {
        var entity = await _db.LeaveRequests.FirstOrDefaultAsync(l => l.Id == id);
        if (entity == null)
            return false;

        entity.Status = request.Status;
        entity.ManagerComments = request.ManagerComments;
        entity.ReviewedById = managerId;
        entity.ReviewedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var leave = await _db.LeaveRequests.FirstOrDefaultAsync(l => l.Id == id);
        if (leave == null || leave.UserId != userId) return false;
        if (leave.Status != LeaveRequestStatus.Pending) return false; // Can only delete pending

        _db.LeaveRequests.Remove(leave);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<LeaveSummaryDto> GetSummaryAsync(int userId, bool isManager)
    {
        IQueryable<LeaveRequest> query = _db.LeaveRequests;
        if (!isManager)
            query = query.Where(l => l.UserId == userId);

        var list = await query.ToListAsync();

        return new LeaveSummaryDto
        {
            TotalRequests = list.Count,
            ApprovedRequests = list.Count(l => l.Status == LeaveRequestStatus.Approved),
            RejectedRequests = list.Count(l => l.Status == LeaveRequestStatus.Rejected),
            PendingRequests = list.Count(l => l.Status == LeaveRequestStatus.Pending),
            TotalDaysRequested = list.Sum(l => Days(l)),
            TotalDaysApproved = list.Where(l => l.Status == LeaveRequestStatus.Approved).Sum(l => Days(l))
        };
    }

    private static LeaveRequestDto MapToDto(LeaveRequest l) => new()
    {
        Id = l.Id,
        UserId = l.UserId,
        UserName = l.User is null ? string.Empty : $"{l.User.FirstName} {l.User.LastName}",
        StartDate = l.StartDate,
        EndDate = l.EndDate,
        DaysCount = Days(l),
        Reason = l.Reason,
        Status = l.Status.ToString(),
        ManagerComments = l.ManagerComments,
        CreatedAt = l.CreatedAt,
        ReviewedAt = l.ReviewedAt
    };

    private static int Days(LeaveRequest l)
    {
        var days = (l.EndDate.Date - l.StartDate.Date).Days + 1;
        return days < 0 ? 0 : days;
    }
}