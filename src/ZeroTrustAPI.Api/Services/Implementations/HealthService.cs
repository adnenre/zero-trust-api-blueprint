using ZeroTrustAPI.Api.Services.Interfaces;

namespace ZeroTrustAPI.Api.Services.Implementations;

public class HealthService : IHealthService
{
    public Task<HealthStatus> GetHealthAsync()
    {
        return Task.FromResult(new HealthStatus("healthy", DateTime.UtcNow));
    }
}