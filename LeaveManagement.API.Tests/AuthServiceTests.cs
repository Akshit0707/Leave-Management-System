using Xunit;
using LeaveManagement.API.Services;
using LeaveManagement.API.DTOs;
using Moq;
using LeaveManagement.API.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using LeaveManagement.API.Models;
using System.Threading.Tasks;

namespace LeaveManagement.API.Tests;

public class AuthServiceTests
{
    private readonly AuthService _authService;
    private readonly AppDbContext _dbContext;
    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _dbContext = new AppDbContext(options);
        var config = new ConfigurationBuilder().AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("Jwt:Key", "testkeytestkeytestkeytestkeytestkeytestkeytestkeytestkey"),
            new KeyValuePair<string, string>("Jwt:Issuer", "TestIssuer"),
            new KeyValuePair<string, string>("Jwt:Audience", "TestAudience"),
            new KeyValuePair<string, string>("Jwt:ExpiresMinutes", "60")
        }).Build();
        _authService = new AuthService(_dbContext, config);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnNull_WhenEmailExists()
    {
        _dbContext.Users.Add(new User { Email = "test@example.com", PasswordHash = "hash", FirstName = "A", LastName = "B", Role = UserRole.Employee });
        await _dbContext.SaveChangesAsync();
        var req = new RegisterRequest { Email = "test@example.com", Password = "pass", FirstName = "A", LastName = "B", Role = 0 };
        var result = await _authService.RegisterAsync(req);
        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenValid()
    {
        var req = new RegisterRequest { Email = "new@example.com", Password = "pass", FirstName = "A", LastName = "B", Role = 0 };
        var result = await _authService.RegisterAsync(req);
        Assert.NotNull(result);
        Assert.Equal("new@example.com", result.Email);
    }
}
