using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LeaveManagement.API.DTOs;
using LeaveManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeavesController : ControllerBase
{
    private readonly ILeaveService _service;

    public LeavesController(ILeaveService service)
    {
        _service = service;
    }

    private int GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier)
                 ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return int.Parse(id ?? throw new InvalidOperationException("User id missing"));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLeaveRequest request)
    {
        var result = await _service.CreateAsync(GetUserId(), request);
        if (result == null) return BadRequest("Invalid date range");
        return Ok(result);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> Mine()
    {
        var result = await _service.GetMyAsync(GetUserId());
        return Ok(result);
    }

    [Authorize(Roles = "Manager")]
    [HttpGet("pending")]
    public async Task<IActionResult> Pending()
    {
        var result = await _service.GetPendingAsync();
        return Ok(result);
    }

    [Authorize(Roles = "Manager")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateLeaveRequest request)
    {
        var ok = await _service.UpdateStatusAsync(id, request, GetUserId());
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        var isManager = User.IsInRole("Manager");
        var result = await _service.GetSummaryAsync(GetUserId(), isManager);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteLeave(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var ok = await _service.DeleteAsync(id, userId);
        if (!ok) return NotFound();
        return NoContent();
    }
}
