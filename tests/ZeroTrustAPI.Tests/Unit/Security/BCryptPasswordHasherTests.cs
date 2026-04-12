using ZeroTrustAPI.Api.Security;
using Xunit;

namespace ZeroTrustAPI.Tests.Unit.Security;

public class BCryptPasswordHasherTests
{
    [Fact]
    public void Hash_ReturnsHash_ThatCanBeVerified()
    {
        var hasher = new BCryptPasswordHasher();
        var password = "mySecret123";
        var hash = hasher.Hash(password);
        Assert.True(hasher.Verify(password, hash));
    }

    [Fact]
    public void Verify_WrongPassword_ReturnsFalse()
    {
        var hasher = new BCryptPasswordHasher();
        var hash = hasher.Hash("correct");
        Assert.False(hasher.Verify("wrong", hash));
    }
}