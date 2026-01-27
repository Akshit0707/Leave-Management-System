using Xunit;
using Moq;
using LeaveManagement.API.Controllers;
using LeaveManagement.API.Services;
using LeaveManagement.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LeaveManagement.API.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_NullRequest_ReturnsBadRequest()
        {
            var result = await _controller.Register(null);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Request body cannot be null", badRequest.Value.ToString());
        }

        [Fact]
        public async Task Register_ExistingEmail_ReturnsBadRequest()
        {
            _authServiceMock.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequest>())).ReturnsAsync((AuthResponse)null);
            var result = await _controller.Register(new RegisterRequest { Email = "exists@example.com" });
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("already exists", badRequest.Value.ToString());
        }

        [Fact]
        public async Task Register_Exception_Returns500()
        {
            _authServiceMock.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequest>())).ThrowsAsync(new System.Exception("fail"));
            var result = await _controller.Register(new RegisterRequest { Email = "fail@example.com" });
            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        [Fact]
        public async Task Login_NullRequest_ReturnsBadRequest()
        {
            var result = await _controller.Login(null);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Request body cannot be null", badRequest.Value.ToString());
        }

        [Fact]
        public async Task Login_Exception_Returns500()
        {
            _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>())).ThrowsAsync(new System.Exception("fail"));
            var result = await _controller.Login(new LoginRequest { Email = "fail@example.com" });
            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        [Fact]
        public async Task RequestPasswordReset_Exception_Returns500()
        {
            _authServiceMock.Setup(s => s.RequestPasswordResetAsync(It.IsAny<string>())).ThrowsAsync(new System.Exception("fail"));
            var dto = new LoginRequest { Email = "fail@example.com" };
            var result = await _controller.RequestPasswordReset(dto);
            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }

        [Fact]
        public async Task DeletePasswordReset_ReturnsOk()
        {
            _authServiceMock.Setup(s => s.DeletePasswordResetRequestAsync(It.IsAny<int>())).ReturnsAsync(true);
            var result = await _controller.DeletePasswordReset(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetAllPasswordResets_ReturnsOk()
        {
            _authServiceMock.Setup(s => s.GetAllPasswordResetRequestsAsync()).ReturnsAsync(new List<object>());
            var result = await _controller.GetAllPasswordResets();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RejectPasswordReset_ReturnsOk()
        {
            _authServiceMock.Setup(s => s.RejectPasswordResetAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);
            var dto = new PasswordResetActionDto { RequestId = 1, Comment = "reason" };
            var result = await _controller.RejectPasswordReset(dto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetPendingPasswordResets_ReturnsOk()
        {
            _authServiceMock.Setup(s => s.GetPendingPasswordResetRequestsAsync()).ReturnsAsync(new List<LeaveManagement.API.Models.PasswordResetRequest>());
            var result = await _controller.GetPendingPasswordResets();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ApprovePasswordReset_ReturnsOk()
        {
            _authServiceMock.Setup(s => s.ApprovePasswordResetAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);
            var dto = new PasswordResetActionDto { RequestId = 1, Comment = "reason" };
            var result = await _controller.ApprovePasswordReset(dto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CompletePasswordReset_ReturnsOk()
        {
            _authServiceMock.Setup(s => s.CompletePasswordResetAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);
            var dto = new CompletePasswordResetDto { RequestId = 1, NewPassword = "pass" };
            var result = await _controller.CompletePasswordReset(dto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_ReturnsOk()
        {
            _authServiceMock.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequest>())).ReturnsAsync(new AuthResponse());
            var result = await _controller.Register(new RegisterRequest());
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsOk()
        {
            _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>())).ReturnsAsync(new AuthResponse());
            var result = await _controller.Login(new LoginRequest());
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RequestPasswordReset_ReturnsOk()
        {
            _authServiceMock.Setup(s => s.RequestPasswordResetAsync(It.IsAny<string>())).ReturnsAsync(true);
            var dto = new LoginRequest { Email = "test@example.com" };
            var result = await _controller.RequestPasswordReset(dto);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
