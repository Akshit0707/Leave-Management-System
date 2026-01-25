using LeaveManagement.API.Data;
using LeaveManagement.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    public UsersController(AppDbContext db)
    {
        _db = db;
    }


    [HttpGet("managers")]
    public async Task<IActionResult> GetManagers()
    {
        var managers = await _db.Users
            .Where(u => u.Role == UserRole.Manager)
            .Select(u => new { u.Id, Name = u.FirstName + " " + u.LastName, u.Email })
            .ToListAsync();
        return Ok(managers);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddUser([FromBody] User user)
    {
        if (user == null) return BadRequest("User data is required");
        if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName))
            return BadRequest("Missing required fields");
        if (await _db.Users.AnyAsync(u => u.Email == user.Email))
            return BadRequest("Email already exists");
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Default@123"); // Set a default password
        user.CreatedAt = DateTime.UtcNow;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Ok(user);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditUser(int id, [FromBody] User updated)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();
        user.FirstName = updated.FirstName;
        user.LastName = updated.LastName;
        user.Email = updated.Email;
        user.Role = updated.Role;
        user.ManagerId = updated.ManagerId;
        await _db.SaveChangesAsync();
        return Ok(user);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _db.Users
            .Select(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.Role, u.ManagerId })
            .ToListAsync();
        return Ok(users);
    }
}
