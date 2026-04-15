using System.Net;
using System.Net.Http.Json;
using Xunit;
using ZeroTrustAPI.Tests.Helpers;
using ZeroTrustAPI.Api.DTOs;
using ZeroTrustAPI.Api.Entities;
using ZeroTrustAPI.Api.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace ZeroTrustAPI.Tests.Integration.Controllers;

public class ApiKeysControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ApiKeysControllerTests(CustomWebApplicationFactory factory)
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
    public async Task CreateApiKey_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.PostAsync("/api/apikeys", null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateApiKey_WithToken_ReturnsApiKey()
    {
        var auth = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.PostAsync("/api/apikeys", null);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var error = await response.Content.ReadAsStringAsync();
            Assert.Fail($"Expected OK but got {response.StatusCode}. Error: {error}");
        }
        // ... rest
    }
    [Fact]
    public async Task GetUserKeys_ReturnsList()
    {
        var auth = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Create a key first
        await _client.PostAsync("/api/apikeys", null);
        var response = await _client.GetAsync("/api/apikeys");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var keys = await response.Content.ReadFromJsonAsync<List<ApiKeyDto>>();
        Assert.NotNull(keys);
        Assert.Single(keys);
    }
}