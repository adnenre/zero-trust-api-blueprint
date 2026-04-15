using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ZeroTrustAPI.Api.Data;
using System;
using System.Linq;

namespace ZeroTrustAPI.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    static CustomWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("TESTING", "true");

    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "superSecretKeyAtLeast32CharsLong123!",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove real DbContext and replace with in‑memory
            var dbDescriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                            d.ServiceType == typeof(AppDbContext))
                .ToList();
            foreach (var descriptor in dbDescriptors)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase")
                       .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }
}