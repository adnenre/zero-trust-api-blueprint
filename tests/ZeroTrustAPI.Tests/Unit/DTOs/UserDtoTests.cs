using System;
using System.Collections.Generic;
using Xunit;
using ZeroTrustAPI.Api.DTOs;

namespace ZeroTrustAPI.Tests.Unit.DTOs;

public class UserDtoTests
{
    [Fact]
    public void UserDto_CanBeCreated_WithObjectInitializer()
    {
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var lastLogin = now.AddDays(-1);
        var roles = new List<string> { "Admin", "User" };

        var dto = new UserDto
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            CreatedAt = now,
            LastLoginAt = lastLogin,
            IsActive = true,
            Roles = roles
        };

        Assert.Equal(userId, dto.Id);
        Assert.Equal("testuser", dto.Username);
        Assert.Equal("test@example.com", dto.Email);
        Assert.Equal(now, dto.CreatedAt);
        Assert.Equal(lastLogin, dto.LastLoginAt);
        Assert.True(dto.IsActive);
        Assert.Equal(roles, dto.Roles);
    }

    [Fact]
    public void UserDto_DefaultValues_AreDefault()
    {
        var dto = new UserDto();
        Assert.Equal(Guid.Empty, dto.Id);
        Assert.Equal(string.Empty, dto.Username);
        Assert.Equal(string.Empty, dto.Email);
        Assert.Equal(default(DateTime), dto.CreatedAt);
        Assert.Null(dto.LastLoginAt);
        Assert.False(dto.IsActive);
        Assert.Empty(dto.Roles);
    }
}