using Microsoft.AspNetCore.Mvc;
using Moq;
using ZeroTrustAPI.Api.Controllers;
using ZeroTrustAPI.Api.Services.Interfaces;
using Xunit;

namespace ZeroTrustAPI.Tests.Unit.Controllers;

public class HealthControllerTests
{
    [Fact]
    public async Task Get_Returns_Ok_With_HealthStatus_From_Service()
    {
        // Arrange
        var expectedStatus = new HealthStatus("healthy", DateTime.UtcNow);
        var mockService = new Mock<IHealthService>();
        mockService.Setup(s => s.GetHealthAsync()).ReturnsAsync(expectedStatus);
        var controller = new HealthController(mockService.Object);

        // Act
        var result = await controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualStatus = Assert.IsType<HealthStatus>(okResult.Value);
        Assert.Equal(expectedStatus.Status, actualStatus.Status);
    }
}