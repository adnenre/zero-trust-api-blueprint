using System.Net;
using System.Net.Http.Json;
using Xunit;
using ZeroTrustAPI.Tests.Helpers;
using ZeroTrustAPI.Api.DTOs;           // AuthResult, LoginRequest, SessionDto
using ZeroTrustAPI.Api.Data;            // AppDbContext
using ZeroTrustAPI.Api.Entities;        // Session
using Microsoft.EntityFrameworkCore;    // FirstAsync
using Microsoft.Extensions.DependencyInjection;

namespace ZeroTrustAPI.Tests.Integration.Controllers;

public class SessionsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;   // ✅ store factory reference

    public SessionsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<AuthResult> RegisterAndLoginAsync()
    {
        var registerRequest = TestData.GetValidRegisterRequest();
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Username = registerRequest.Username,
            Password = registerRequest.Password
        });
        return (await loginResponse.Content.ReadFromJsonAsync<AuthResult>())!;
    }

    [Fact]
    public async Task GetSessions_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/sessions");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetSessions_WithToken_ReturnsSessions()
    {
        var auth = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.GetAsync("/api/sessions");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var sessions = await response.Content.ReadFromJsonAsync<List<SessionDto>>();
        Assert.NotNull(sessions);
        // The test may pass even if list is empty. If you expect at least one session,
        // you need to seed a session (like in the next test) or modify your login to create one.
    }

    [Fact]
    public async Task RevokeSession_ValidId_ReturnsNoContent()
    {
        var auth = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Manually create a session in the in‑memory database
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await db.Users.FirstAsync(u => u.Id == auth.UserId);
        var session = new Session
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };
        db.Sessions.Add(session);
        await db.SaveChangesAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/sessions/{session.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task RevokeSession_InvalidId_ReturnsNotFound()
    {
        var auth = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.DeleteAsync($"/api/sessions/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}