namespace ZeroTrustAPI.Api.Services.Interfaces;

public interface IHealthService
{
    Task<HealthStatus> GetHealthAsync();
}

public record HealthStatus(string Status, DateTime Timestamp);