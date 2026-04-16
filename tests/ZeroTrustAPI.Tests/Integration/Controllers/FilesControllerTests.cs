using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;               // for ReadFromJsonAsync
using Xunit;
using ZeroTrustAPI.Tests.Helpers;
using ZeroTrustAPI.Api.DTOs;             // for AuthResult, LoginRequest

namespace ZeroTrustAPI.Tests.Integration.Controllers;

public class FilesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    public FilesControllerTests(CustomWebApplicationFactory factory)
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
    public async Task UploadFile_WithoutToken_ReturnsUnauthorized()
    {
        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(new byte[] { 1, 2, 3 }), "file", "test.txt");
        var response = await _client.PostAsync("/api/files/upload", content);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UploadFile_WithToken_ReturnsOk()
    {
        var auth = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3, 4 });
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        var content = new MultipartFormDataContent();
        content.Add(fileContent, "file", "test.txt");

        var response = await _client.PostAsync("/api/files/upload", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.NotNull(result);
        Assert.Contains("id", result.Keys, StringComparer.OrdinalIgnoreCase);       // ✅ matches controller: new { result.Id, ... }
        Assert.Contains(result.Keys, key => string.Equals(key, "FileName", StringComparison.OrdinalIgnoreCase));
        Assert.Equal("test.txt", result["fileName"]?.ToString());
    }

    [Fact]
    public async Task UploadFile_NoFile_ReturnsBadRequest()
    {
        var auth = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var content = new MultipartFormDataContent();
        var response = await _client.PostAsync("/api/files/upload", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}