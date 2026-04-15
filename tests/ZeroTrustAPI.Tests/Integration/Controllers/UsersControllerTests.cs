using System.Net;
using System.Net.Http.Json;
using Xunit;
using ZeroTrustAPI.Api.DTOs;
using ZeroTrustAPI.Tests.Helpers;
using System.Linq;   // for Contains

namespace ZeroTrustAPI.Tests.Integration.Controllers;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    public UsersControllerTests(CustomWebApplicationFactory factory)
    {
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
    public async Task GetAll_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithToken_ReturnsUsers()
    {
        var auth = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        Assert.NotNull(users);
        // Check that the user we created is in the list (instead of expecting exactly one)
        Assert.Contains(users, u => u.Id == auth.UserId);
    }

    [Fact]
    public async Task GetById_ValidId_ReturnsUser()
    {
        var auth = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.GetAsync($"/api/users/{auth.UserId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.Equal(auth.UserId, user!.Id);
    }

    [Fact]
    public async Task GetById_InvalidId_ReturnsNotFound()
    {
        var auth = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.GetAsync($"/api/users/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetMe_ReturnsCurrentUser()
    {
        var auth = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.GetAsync("/api/users/me");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.Equal(auth.UserId, user!.Id);
    }

    [Fact]
    public async Task UpdateUser_ValidData_ReturnsNoContent()
    {
        var auth = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var updateRequest = new UpdateUserRequest { Email = "updated@example.com" };
        var response = await _client.PutAsJsonAsync($"/api/users/{auth.UserId}", updateRequest);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify change
        var getResponse = await _client.GetAsync($"/api/users/{auth.UserId}");
        var user = await getResponse.Content.ReadFromJsonAsync<UserDto>();
        Assert.Equal("updated@example.com", user!.Email);
    }

    [Fact]
    public async Task UpdateUser_WrongUser_ReturnsNotFound()
    {
        var auth = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var updateRequest = new UpdateUserRequest { Email = "updated@example.com" };
        var response = await _client.PutAsJsonAsync($"/api/users/{Guid.NewGuid()}", updateRequest);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ValidUser_ReturnsNoContent()
    {
        var auth = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.DeleteAsync($"/api/users/{auth.UserId}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify gone
        var getResponse = await _client.GetAsync($"/api/users/{auth.UserId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_Unauthorized_ReturnsForbidden()
    {
        var auth = await RegisterAndLoginAsync();
        // Create second user
        var registerRequest2 = new RegisterRequest
        {
            Username = "otheruser",
            Email = "other@example.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest2);
        var loginResponse2 = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Username = "otheruser",
            Password = "Test123!"
        });
        var auth2 = await loginResponse2.Content.ReadFromJsonAsync<AuthResult>();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.DeleteAsync($"/api/users/{auth2!.UserId}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}