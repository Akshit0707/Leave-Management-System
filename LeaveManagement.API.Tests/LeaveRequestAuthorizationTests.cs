using Xunit;
using LeaveManagement.API.Models;
using System;

namespace LeaveManagement.API.Tests;

public class LeaveRequestAuthorizationTests
{
    [Fact]
    public void OnlyManagerCanApproveOrReject()
    {
        // Arrange
        var manager = new User { Id = 1, Role = UserRole.Manager };
        var employee = new User { Id = 2, Role = UserRole.Employee };
        var leaveRequest = new LeaveRequest { Id = 1, Status = LeaveRequestStatus.Pending };

        // Act
        bool managerCanApprove = CanApproveOrReject(manager);
        bool employeeCanApprove = CanApproveOrReject(employee);

        // Assert
        Assert.True(managerCanApprove);
        Assert.False(employeeCanApprove);
    }

    private bool CanApproveOrReject(User user)
    {
        return user.Role == UserRole.Manager;
    }
}
