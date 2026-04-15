using System;
using System.Threading.Tasks;

namespace ZeroTrustAPI.Api.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for health check services.
    /// </summary>
    public interface IHealthService
    {
        /// <summary>
        /// Retrieves the current health status of the application.
        /// </summary>
        /// <returns>A <see cref="HealthStatus"/> object containing status and timestamp.</returns>
        Task<HealthStatus> GetHealthAsync();
    }

    /// <summary>
    /// Represents the health status of the application at a specific moment.
    /// </summary>
    /// <param name="Status">String indicating health (e.g., "healthy", "unhealthy").</param>
    /// <param name="Timestamp">UTC timestamp when the status was recorded.</param>
    public record HealthStatus(string Status, DateTime Timestamp)
    {
        /// <summary>
        /// Computed property that returns true if the status indicates healthy.
        /// Case‑insensitive comparison.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore] // Prevents this property from appearing in JSON responses
        public bool IsHealthy => Status.Equals("healthy", StringComparison.OrdinalIgnoreCase);
    }
}