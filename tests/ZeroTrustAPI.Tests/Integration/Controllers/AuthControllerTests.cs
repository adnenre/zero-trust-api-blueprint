using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using ZeroTrustAPI.Api.Data;
using ZeroTrustAPI.Api.DTOs;
using ZeroTrustAPI.Api.Entities;
using ZeroTrustAPI.Api.Security;
using Xunit;

namespace ZeroTrustAPI.Tests.Integration.Controllers;

public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        Environment.SetEnvironmentVariable("TESTING", "true");

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove real DbContext and replace with in-memory
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestAuthDb"));

                // Seed a test user
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();

                var hasher = new BCryptPasswordHasher();
                if (!db.Users.Any(u => u.Username == "testuser"))
                {
                    db.Users.Add(new User
                    {
                        Username = "testuser",
                        Email = "test@example.com",
                        PasswordHash = hasher.Hash("password123")
                    });
                    db.SaveChanges();
                }
            });
        });
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var request = new LoginRequest("testuser", "password123");
        var response = await client.PostAsJsonAsync("/api/auth/login", request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthResult>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("testuser", result.User?.Username);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var request = new LoginRequest("wrong", "wrong");
        var response = await client.PostAsJsonAsync("/api/auth/login", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}