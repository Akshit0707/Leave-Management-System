using Xunit;
using LeaveManagement.API.Controllers;
using LeaveManagement.API.Services;
using LeaveManagement.API.DTOs;
using LeaveManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveManagement.API.Tests;

public class LeavesControllerTests
{
    private readonly LeavesController _controller;
    private readonly Mock<ILeaveService> _leaveServiceMock = new();

    public LeavesControllerTests()
    {
        _controller = new LeavesController(_leaveServiceMock.Object);
    }

    [Fact]
    public async Task Create_ReturnsOk_WhenLeaveCreated()
    {
        var request = new CreateLeaveRequest();
        var leaveDto = new LeaveRequestDto();
        _leaveServiceMock.Setup(s => s.CreateAsync(It.IsAny<int>(), request)).ReturnsAsync(leaveDto);
        var controller = _controller;
        var user = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[] {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "1")
        }, "mock"));
        controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user } };
        var result = await controller.Create(request);
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Mine_ReturnsOkWithList()
    {
        _leaveServiceMock.Setup(s => s.GetMyAsync(It.IsAny<int>())).ReturnsAsync(new List<LeaveRequestDto>());
        var controller = _controller;
        var user = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[] {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "1")
        }, "mock"));
        controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user } };
        var result = await controller.Mine();
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsAssignableFrom<IEnumerable<LeaveRequestDto>>(okResult.Value);
    }
}
