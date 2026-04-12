using ZeroTrustAPI.Api.Entities;
using Xunit;

namespace ZeroTrustAPI.Tests.Unit.Entities;

public class UserEntityTests
{
    [Fact]
    public void UserProperties_SetAndGet()
    {
        var user = new User
        {
            Id = 1,
            Username = "john",
            Email = "john@example.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow
        };
        Assert.Equal(1, user.Id);
        Assert.Equal("john", user.Username);
        Assert.Equal("john@example.com", user.Email);
        Assert.Equal("hash", user.PasswordHash);
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
    }
}