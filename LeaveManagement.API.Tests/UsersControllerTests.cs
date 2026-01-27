using Xunit;
using Moq;
using LeaveManagement.API.Controllers;
using LeaveManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeaveManagement.API.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.API.Tests
{
    public class UsersControllerTests
    {
        private UsersController CreateController(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var dbContext = new AppDbContext(options);
            return new UsersController(dbContext);
        }

        [Fact]
        public async Task AddUser_NullUser_ReturnsBadRequest()
        {
            var controller = CreateController(nameof(AddUser_NullUser_ReturnsBadRequest));
            var result = await controller.AddUser(null);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("User data is required", badRequest.Value.ToString());
        }

        [Fact]
        public async Task AddUser_MissingFields_ReturnsBadRequest()
        {
            var controller = CreateController(nameof(AddUser_MissingFields_ReturnsBadRequest));
            var user = new User { Id = 10, Email = "", FirstName = "", LastName = "" };
            var result = await controller.AddUser(user);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Missing required fields", badRequest.Value.ToString());
        }

        [Fact]
        public async Task AddUser_DuplicateEmail_ReturnsBadRequest()
        {
            var controller = CreateController(nameof(AddUser_DuplicateEmail_ReturnsBadRequest));
            var dbContext = (AppDbContext)controller.GetType().GetField("_db", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(controller);
            var user = new User { Id = 11, FirstName = "Test", LastName = "User", Email = "dup@example.com", PasswordHash = "hash" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            var duplicate = new User { Id = 12, FirstName = "Test2", LastName = "User2", Email = "dup@example.com", PasswordHash = "hash" };
            var result = await controller.AddUser(duplicate);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Email already exists", badRequest.Value.ToString());
        }

        [Fact]
        public async Task EditUser_NotFound_ReturnsNotFound()
        {
            var controller = CreateController(nameof(EditUser_NotFound_ReturnsNotFound));
            var updated = new User { Id = 99, FirstName = "Not", LastName = "Found", Email = "notfound@example.com", PasswordHash = "hash" };
            var result = await controller.EditUser(99, updated);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteUser_NotFound_ReturnsNotFound()
        {
            var controller = CreateController(nameof(DeleteUser_NotFound_ReturnsNotFound));
            var result = await controller.DeleteUser(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetManagers_ReturnsOk()
        {
            var controller = CreateController(nameof(GetManagers_ReturnsOk));
            var result = await controller.GetManagers();
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AddUser_ReturnsOk()
        {
            var controller = CreateController(nameof(AddUser_ReturnsOk));
            var user = new User { Id = 1, FirstName = "Test", LastName = "User", Email = "test1@example.com", PasswordHash = "hash" };
            var result = await controller.AddUser(user);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task EditUser_ReturnsOk()
        {
            var controller = CreateController(nameof(EditUser_ReturnsOk));
            var dbContext = (AppDbContext)controller.GetType().GetField("_db", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(controller);
            var user = new User { Id = 2, FirstName = "Edit", LastName = "User", Email = "edit@example.com", PasswordHash = "hash" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            var updated = new User { Id = 2, FirstName = "Updated", LastName = "User", Email = "edit@example.com", PasswordHash = "hash" };
            var result = await controller.EditUser(2, updated);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsOk()
        {
            var controller = CreateController(nameof(DeleteUser_ReturnsOk));
            var dbContext = (AppDbContext)controller.GetType().GetField("_db", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(controller);
            var user = new User { Id = 3, FirstName = "Delete", LastName = "User", Email = "delete@example.com", PasswordHash = "hash" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            var result = await controller.DeleteUser(3);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk()
        {
            var controller = CreateController(nameof(GetAllUsers_ReturnsOk));
            var result = await controller.GetAllUsers();
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
