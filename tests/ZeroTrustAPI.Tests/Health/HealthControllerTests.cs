using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ZeroTrustAPI.Api;
using Xunit;

namespace ZeroTrustAPI.Tests.Health;

// Define a record matching the HealthController response
public record HealthResponse(string status, DateTime timestamp);

public class HealthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_Endpoint_Returns_Ok_With_Status_Healthy()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Deserialize into the HealthResponse record
        var result = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.NotNull(result);
        Assert.Equal("healthy", result?.status);
        Assert.True(result?.timestamp != default);
    }
}