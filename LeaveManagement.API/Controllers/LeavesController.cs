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
    private readonly ILogger<LeavesController> _logger;

    public LeavesController(ILeaveService service, ILogger<LeavesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    private int GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier)
                 ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        
        if (string.IsNullOrEmpty(id))
        {
            _logger.LogError("User ID not found in token claims");
            throw new InvalidOperationException("User id missing from token");
        }
        
        return int.Parse(id);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeaveRequest request)
    {
        try
        {
            _logger.LogInformation("Creating leave request");
            
            var userId = GetUserId();
            _logger.LogInformation($"User ID from token: {userId}");
            
            var result = await _service.CreateAsync(userId, request);
            if (result == null)
            {
                _logger.LogWarning("Invalid date range provided");
                return BadRequest(new { error = "Invalid date range" });
            }
            
            _logger.LogInformation($"Leave created successfully: {result.Id}");
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError($"Operation error: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating leave: {ex.Message}");
            _logger.LogError($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
        }
    }

    [HttpGet("mine")]
    public async Task<IActionResult> Mine()
    {
        try
        {
            var result = await _service.GetMyAsync(GetUserId());
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching leaves: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Manager")]
    [HttpGet("pending")]
    public async Task<IActionResult> Pending()
    {
        try
        {
            var result = await _service.GetPendingAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching pending leaves: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Manager")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateLeaveRequest request)
    {
        try
        {
            var ok = await _service.UpdateStatusAsync(id, request, GetUserId());
            if (!ok) return NotFound(new { error = "Leave request not found" });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating leave status: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        try
        {
            var isManager = User.IsInRole("Manager");
            var result = await _service.GetSummaryAsync(GetUserId(), isManager);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching summary: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteLeave(int id)
    {
        try
        {
            var userId = GetUserId();
            var ok = await _service.DeleteAsync(id, userId);
            if (!ok) return NotFound(new { error = "Leave request not found or cannot be deleted" });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting leave: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
