using LeaveManagement.API.DTOs;
namespace LeaveManagement.API.Services;

public interface ILeaveService
{
    Task<LeaveRequestDto?> CreateAsync(int userId, CreateLeaveRequest request);
    Task<IEnumerable<LeaveRequestDto>> GetMyAsync(int userId);
    Task<IEnumerable<LeaveRequestDto>> GetPendingAsync();
    Task<bool> UpdateStatusAsync(int id, UpdateLeaveRequest request, int managerId);
    Task<LeaveSummaryDto> GetSummaryAsync(int userId, bool isManager);
    Task<bool> DeleteAsync(int id, int userId);
}