using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ZeroTrustAPI.Api;
using Xunit;

namespace ZeroTrustAPI.Tests.Integration.Health;

public record HealthResponse(string status, DateTime timestamp);

public class HealthEndpointIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthEndpointIntegrationTests(WebApplicationFactory<Program> factory)
    {
        // Set environment to "Testing" to avoid running migrations (only in Development)
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ASPNETCORE_ENVIRONMENT", "Testing");
        }).CreateClient();
    }

    [Fact]
    public async Task Health_Endpoint_Returns_Ok_With_Status_Healthy()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.NotNull(result);
        Assert.Equal("healthy", result?.status);
        Assert.True(result?.timestamp != default);
    }
}