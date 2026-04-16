using System;
using System.Threading.Tasks;
using Xunit;
using ZeroTrustAPI.Api.Services.Implementations;
using ZeroTrustAPI.Api.Services.Interfaces;

namespace ZeroTrustAPI.Tests.Unit.Services;

public class HealthServiceTests
{
    [Fact]
    public async Task GetHealthAsync_WhenCalled_ReturnsHealthyStatus()
    {
        // Arrange
        var service = new HealthService();

        // Act
        var result = await service.GetHealthAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("healthy", result.Status);
        Assert.True(result.IsHealthy);
        Assert.True((DateTime.UtcNow - result.Timestamp).TotalSeconds < 5);
    }
}