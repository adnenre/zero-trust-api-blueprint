using System.Net;
using Xunit;

namespace ZeroTrustAPI.Tests.Integration.Controllers;

public class HealthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    public HealthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // For unhealthy scenario, you would need to mock the health check service.
}