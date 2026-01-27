using Xunit;
using LeaveManagement.API.Controllers;
using LeaveManagement.API.Services;
using LeaveManagement.API.DTOs;
using LeaveManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace LeaveManagement.API.Tests
{
public class LeavesControllerTests
{
    private readonly Mock<ILeaveService> _leaveServiceMock = new();
    private readonly LeavesController _controller;

    public LeavesControllerTests()
    {
        _controller = new LeavesController(_leaveServiceMock.Object);
    }



    private LeavesController CreateControllerWithUser(string userId = "1", string[] roles = null)
    {
        var controller = new LeavesController(_leaveServiceMock.Object);
        var claims = new List<System.Security.Claims.Claim> { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userId) };
        if (roles != null)
        {
            foreach (var role in roles)
                claims.Add(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role));
        }
        var user = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims, "mock"));
        controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user } };
        return controller;
    }

    [Fact]
    public async Task GetAll_ManagerRole_ReturnsOk()
    {
        _leaveServiceMock.Setup(s => s.GetAllForManagerAsync(It.IsAny<int>())).ReturnsAsync(new List<LeaveRequestDto>());
        var controller = CreateControllerWithUser("2", new[] { "Manager" });
        var result = await controller.GetAll();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetAll_ManagerRole_ThrowsException_Returns500()
    {
        _leaveServiceMock.Setup(s => s.GetAllForManagerAsync(It.IsAny<int>())).ThrowsAsync(new System.Exception("fail"));
        var controller = CreateControllerWithUser("2", new[] { "Manager" });
        var result = await controller.GetAll();
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidDateRange_ReturnsBadRequest()
    {
        _leaveServiceMock.Setup(s => s.CreateAsync(It.IsAny<int>(), It.IsAny<CreateLeaveRequest>())).ReturnsAsync((LeaveRequestDto)null);
        var controller = CreateControllerWithUser();
        var result = await controller.Create(new CreateLeaveRequest());
        var obj = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Invalid date range", obj.Value.ToString());
    }

    [Fact]
    public async Task Create_ThrowsException_Returns500()
    {
        _leaveServiceMock.Setup(s => s.CreateAsync(It.IsAny<int>(), It.IsAny<CreateLeaveRequest>())).ThrowsAsync(new System.Exception("fail"));
        var controller = CreateControllerWithUser();
        var result = await controller.Create(new CreateLeaveRequest());
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }

    [Fact]
    public async Task Mine_ThrowsException_Returns500()
    {
        _leaveServiceMock.Setup(s => s.GetMyAsync(It.IsAny<int>())).ThrowsAsync(new System.Exception("fail"));
        var controller = CreateControllerWithUser();
        var result = await controller.Mine();
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }

    [Fact]
    public async Task Pending_ManagerRole_ReturnsOk()
    {
        _leaveServiceMock.Setup(s => s.GetPendingForManagerAsync(It.IsAny<int>())).ReturnsAsync(new List<LeaveRequestDto>());
        var controller = CreateControllerWithUser("2", new[] { "Manager" });
        var result = await controller.Pending();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Pending_ManagerRole_ThrowsException_Returns500()
    {
        _leaveServiceMock.Setup(s => s.GetPendingForManagerAsync(It.IsAny<int>())).ThrowsAsync(new System.Exception("fail"));
        var controller = CreateControllerWithUser("2", new[] { "Manager" });
        var result = await controller.Pending();
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_Success_ReturnsNoContent()
    {
        _leaveServiceMock.Setup(s => s.UpdateStatusAsync(It.IsAny<int>(), It.IsAny<UpdateLeaveRequest>(), It.IsAny<int>())).ReturnsAsync(true);
        var controller = CreateControllerWithUser("2", new[] { "Manager" });
        var result = await controller.UpdateStatus(1, new UpdateLeaveRequest());
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateStatus_NotFound_ReturnsNotFound()
    {
        _leaveServiceMock.Setup(s => s.UpdateStatusAsync(It.IsAny<int>(), It.IsAny<UpdateLeaveRequest>(), It.IsAny<int>())).ReturnsAsync(false);
        var controller = CreateControllerWithUser("2", new[] { "Manager" });
        var result = await controller.UpdateStatus(1, new UpdateLeaveRequest());
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateStatus_ThrowsException_Returns500()
    {
        _leaveServiceMock.Setup(s => s.UpdateStatusAsync(It.IsAny<int>(), It.IsAny<UpdateLeaveRequest>(), It.IsAny<int>())).ThrowsAsync(new System.Exception("fail"));
        var controller = CreateControllerWithUser("2", new[] { "Manager" });
        var result = await controller.UpdateStatus(1, new UpdateLeaveRequest());
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }

    [Fact]
    public async Task Summary_ReturnsOk()
    {
        _leaveServiceMock.Setup(s => s.GetSummaryAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(new LeaveSummaryDto());
        var controller = CreateControllerWithUser();
        var result = await controller.Summary();
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Summary_ThrowsException_Returns500()
    {
        _leaveServiceMock.Setup(s => s.GetSummaryAsync(It.IsAny<int>(), It.IsAny<bool>())).ThrowsAsync(new System.Exception("fail"));
        var controller = CreateControllerWithUser();
        var result = await controller.Summary();
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }

    [Fact]
    public async Task DeleteLeave_Success_ReturnsNoContent()
    {
        _leaveServiceMock.Setup(s => s.DeleteAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
        var controller = CreateControllerWithUser();
        var result = await controller.DeleteLeave(1);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteLeave_NotFound_ReturnsNotFound()
    {
        _leaveServiceMock.Setup(s => s.DeleteAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);
        var controller = CreateControllerWithUser();
        var result = await controller.DeleteLeave(1);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteLeave_ThrowsException_Returns500()
    {
        _leaveServiceMock.Setup(s => s.DeleteAsync(It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new System.Exception("fail"));
        var controller = CreateControllerWithUser();
        var result = await controller.DeleteLeave(1);
        var obj = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, obj.StatusCode);
    }
    [Fact]
    public async Task Create_ReturnsOk_WhenLeaveCreated()
    {
        var request = new CreateLeaveRequest();
        var leaveDto = new LeaveRequestDto();
        _leaveServiceMock.Setup(s => s.CreateAsync(It.IsAny<int>(), request)).ReturnsAsync(leaveDto);
        var controller = CreateControllerWithUser();
        var result = await controller.Create(request);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Mine_ReturnsOkWithList()
    {
        _leaveServiceMock.Setup(s => s.GetMyAsync(It.IsAny<int>())).ReturnsAsync(new List<LeaveRequestDto>());
        var controller = CreateControllerWithUser();
        var result = await controller.Mine();
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsAssignableFrom<IEnumerable<LeaveRequestDto>>(okResult.Value);
    }
}
}