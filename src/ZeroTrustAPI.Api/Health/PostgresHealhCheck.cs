using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace ZeroTrustAPI.Api.Health;

public class PostgresHealthCheck : IHealthCheck
{
    private readonly string? _connectionString;

    public PostgresHealthCheck(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            return HealthCheckResult.Unhealthy("PostgreSQL connection string is not configured.");
        }

        try
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync(cancellationToken);
            await using var cmd = new NpgsqlCommand("SELECT 1", conn);
            await cmd.ExecuteScalarAsync(cancellationToken);
            return HealthCheckResult.Healthy("PostgreSQL is ready.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL is unhealthy.", ex);
        }
    }
}