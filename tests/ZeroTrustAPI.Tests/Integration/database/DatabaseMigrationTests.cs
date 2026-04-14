using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit;
using ZeroTrustAPI.Api.Data;   // adjust namespace if needed

namespace ZeroTestApi.Tests.Integration.Database;

public class DatabaseMigrationTests
{
    private readonly string _connectionString;

    public DatabaseMigrationTests()
    {
        _connectionString = "Host=localhost;Database=zero_trust_test;Username=postgres;Password=postgres";
    }

    [Fact]
    public async Task Migrations_CreateExpectedTables()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        await using var dbContext = new AppDbContext(options);
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        var expectedTables = new[]
        {
            "Users", "Roles", "UserRoles",
            "RefreshTokens", "ApiKeys", "Sessions",
            "Devices", "AuditLogs", "UploadedFiles"
        };

        foreach (var table in expectedTables)
        {
            var exists = await TableExists(conn, table);
            Assert.True(exists, $"Table {table} does not exist.");
        }
    }

    private static async Task<bool> TableExists(NpgsqlConnection conn, string tableName)
    {
        var sql = "SELECT COUNT(*) FROM information_schema.tables WHERE table_name = @tableName";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@tableName", tableName.ToLowerInvariant());
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt64(result) > 0;
    }
}