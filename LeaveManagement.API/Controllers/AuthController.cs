
using LeaveManagement.API.DTOs;
using LeaveManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpGet("all-password-resets")]
    public async Task<IActionResult> GetAllPasswordResets()
    {
        var requests = await _auth.GetAllPasswordResetRequestsAsync();
        return Ok(requests);
    }

    [HttpPost("reject-password-reset")]
    public async Task<IActionResult> RejectPasswordReset([FromBody] int requestId)
    {
        var result = await _auth.RejectPasswordResetAsync(requestId);
        if (!result) return BadRequest("Could not reject request.");
        return Ok(new { message = "Request rejected." });
    }

    [HttpGet("pending-password-resets")]
    public async Task<IActionResult> GetPendingPasswordResets()
    {
        var requests = await _auth.GetPendingPasswordResetRequestsAsync();
        return Ok(requests);
    }

    [HttpPost("approve-password-reset")]
    public async Task<IActionResult> ApprovePasswordReset([FromBody] int requestId)
    {
        var result = await _auth.ApprovePasswordResetAsync(requestId);
        if (!result) return BadRequest("Could not approve request.");
        return Ok(new { message = "Request approved." });
    }

    [HttpPost("complete-password-reset")]
    public async Task<IActionResult> CompletePasswordReset([FromBody] CompletePasswordResetDto dto)
    {
        var result = await _auth.CompletePasswordResetAsync(dto.RequestId, dto.NewPassword);
        if (!result) return BadRequest("Could not complete password reset.");
        return Ok(new { message = "Password reset completed." });
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (request == null)
            {
                Console.WriteLine("Register failed: request body is null");
                return BadRequest("Request body cannot be null.");
            }
            Console.WriteLine("=== Register attempt ===");
            Console.WriteLine($"Email: {request.Email}, Role: {request.Role}, ManagerId: {request.ManagerId}");
            var response = await _auth.RegisterAsync(request);
            if (response == null)
            {
                Console.WriteLine("Register failed: email exists");
                return BadRequest("User with this email already exists.");
            }
            Console.WriteLine("Register success");
            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Register error: {ex.Message}");
            if (ex.InnerException != null) Console.WriteLine($"Inner: {ex.InnerException.Message}");
            Console.WriteLine(ex.StackTrace);
            return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (request == null)
            {
                Console.WriteLine("Login failed: request body is null");
                return BadRequest("Request body cannot be null.");
            }
            Console.WriteLine($"=== Login attempt === {request.Email}");
            var response = await _auth.LoginAsync(request);
            if (response == null)
            {
                Console.WriteLine("Login failed: invalid creds");
                return Unauthorized("Invalid email or password.");
            }
            Console.WriteLine("Login success");
            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            if (ex.InnerException != null) Console.WriteLine($"Inner: {ex.InnerException.Message}");
            Console.WriteLine(ex.StackTrace);
            return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
        }
    }

    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] LoginRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email is required.");
            var result = await _auth.RequestPasswordResetAsync(request.Email);
            if (!result)
                return BadRequest("Could not submit password reset request.");
            return Ok(new { message = "Password reset request submitted. Await admin approval." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}