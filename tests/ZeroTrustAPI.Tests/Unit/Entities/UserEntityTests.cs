using System;
using Xunit;
using ZeroTrustAPI.Api.Entities;

namespace ZeroTrustAPI.Tests.Unit.Entities;

public class UserEntityTests
{
    [Fact]
    public void User_CanBeCreated_WithValidProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        // Act
        var user = new User
        {
            Id = userId,
            Username = "john_doe",
            Email = "john@example.com",
            PasswordHash = "hashed_password_123",
            CreatedAt = now
        };

        // Assert
        Assert.Equal(userId, user.Id);
        Assert.Equal("john_doe", user.Username);
        Assert.Equal("john@example.com", user.Email);
        Assert.Equal("hashed_password_123", user.PasswordHash);
        Assert.Equal(now, user.CreatedAt);
    }

    [Fact]
    public void User_DefaultValues_AreSet()
    {
        // Act
        var user = new User();

        // Assert
        Assert.Equal(Guid.Empty, user.Id);
        Assert.Equal(string.Empty, user.Username);   // Changed from Assert.Null
        Assert.Equal(string.Empty, user.Email);      // Changed from Assert.Null
        Assert.Equal(string.Empty, user.PasswordHash); // Changed from Assert.Null
        Assert.Equal(default(DateTime), user.CreatedAt);
    }

    [Fact]
    public void User_CanUpdateProperties()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "old_name",
            Email = "old@example.com",
            PasswordHash = "old_hash",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        user.Username = "new_name";
        user.Email = "new@example.com";

        // Assert
        Assert.Equal("new_name", user.Username);
        Assert.Equal("new@example.com", user.Email);
    }

    [Fact]
    public void User_Id_IsUnique()
    {
        // Arrange
        var user1 = new User { Id = Guid.NewGuid() };
        var user2 = new User { Id = Guid.NewGuid() };

        // Assert
        Assert.NotEqual(user1.Id, user2.Id);
    }
}