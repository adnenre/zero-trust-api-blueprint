using ZeroTrustAPI.Api.Services.Implementations;
using Xunit;

namespace ZeroTrustAPI.Tests.Unit.Services;

public class HealthServiceTests
{
    [Fact]
    public async Task GetHealthAsync_ReturnsHealthyStatus()
    {
        var service = new HealthService();
        var result = await service.GetHealthAsync();
        Assert.Equal("healthy", result.Status);
        Assert.True((DateTime.UtcNow - result.Timestamp).TotalSeconds < 1);
    }
}