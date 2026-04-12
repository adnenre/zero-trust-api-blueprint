using ZeroTrustAPI.Api.DTOs;
using Xunit;

namespace ZeroTrustAPI.Tests.Unit.DTOs;

public class UserDtoTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var dto = new UserDto(42, "testuser");
        Assert.Equal(42, dto.Id);
        Assert.Equal("testuser", dto.Username);
    }
}