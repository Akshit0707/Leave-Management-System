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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _auth.RegisterAsync(request);
        if (response == null)
            return BadRequest("User with this email already exists.");

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _auth.LoginAsync(request);
        if (response == null)
            return Unauthorized("Invalid email or password.");

        return Ok(response);
    }
}