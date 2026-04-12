using Microsoft.EntityFrameworkCore;
using ZeroTrustAPI.Api.Data;
using ZeroTrustAPI.Api.Entities;
using ZeroTrustAPI.Api.Repositories.Implementations;
using Xunit;

namespace ZeroTrustAPI.Tests.Unit.Repositories;

public class UserRepositoryTests
{
    private AppDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByUsernameAsync_UserExists_ReturnsUser()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var user = new User { Username = "john", Email = "john@test.com", PasswordHash = "hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        var repo = new UserRepository(context);

        // Act
        var result = await repo.GetByUsernameAsync("john");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("john", result.Username);
        Assert.Equal("john@test.com", result.Email);
    }

    [Fact]
    public async Task GetByUsernameAsync_UserDoesNotExist_ReturnsNull()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repo = new UserRepository(context);

        // Act
        var result = await repo.GetByUsernameAsync("unknown");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsUserToDatabase()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var repo = new UserRepository(context);
        var user = new User { Username = "newuser", Email = "new@test.com", PasswordHash = "hash" };

        // Act
        await repo.AddAsync(user);
        var saved = await context.Users.FirstOrDefaultAsync(u => u.Username == "newuser");

        // Assert
        Assert.NotNull(saved);
        Assert.Equal("new@test.com", saved.Email);
    }
}