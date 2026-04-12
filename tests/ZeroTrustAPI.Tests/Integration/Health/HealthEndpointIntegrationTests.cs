using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using ZeroTrustAPI.Api.Services.Interfaces;
using Xunit;

namespace ZeroTrustAPI.Tests.Integration.Health;

public class HealthEndpointIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthEndpointIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IHealthService>();
                var mock = new Mock<IHealthService>();
                mock.Setup(s => s.GetHealthAsync())
                    .ReturnsAsync(new HealthStatus("Healthy", DateTime.UtcNow));
                services.AddScoped(_ => mock.Object);
            });
        });
    }

    [Fact]
    public async Task Health_Endpoint_Returns_Ok_With_Status_Healthy()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<HealthStatus>();
        Assert.NotNull(result);
        Assert.Equal("Healthy", result.Status);
    }
}