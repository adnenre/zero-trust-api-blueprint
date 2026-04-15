using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit;
using ZeroTrustAPI.Api.Data;

namespace ZeroTrustAPI.Tests.Integration.Database;

public class DatabaseMigrationTests
{
    private readonly string _connectionString;

    public DatabaseMigrationTests()
    {
        // Use a separate test database to avoid conflicts
        _connectionString = "Host=localhost;Database=zero_trust_test_migrations;Username=postgres;Password=postgres";
    }

    [Fact(Skip = "This test requires a real PostgreSQL database and is skipped during CI/in-memory tests.")]
    public async Task Migrations_CreateExpectedTables()
    {
        // Skip this test when running in‑memory (integration tests with TESTING=true)
        if (Environment.GetEnvironmentVariable("TESTING") == "true")
        {
            // Return without failing – effectively skip the test
            return;
        }

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        await using var dbContext = new AppDbContext(options);
        await dbContext.Database.EnsureDeletedAsync();   // fresh start
        await dbContext.Database.MigrateAsync();         // apply all migrations

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