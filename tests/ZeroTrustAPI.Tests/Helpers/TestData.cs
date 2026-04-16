using ZeroTrustAPI.Api.DTOs;
using ZeroTrustAPI.Api.Entities;

namespace ZeroTrustAPI.Tests.Helpers;

public static class TestData
{
    public static User GetValidUser() => new()
    {
        Id = Guid.NewGuid(),
        Username = "testuser",
        Email = "test@example.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!")
    };
    public static RegisterRequest GetValidRegisterRequest()
    {
        var unique = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
        return new RegisterRequest
        {
            Username = $"testuser_{unique}",
            Email = $"test{unique}@example.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!"
        };
    }

    public static LoginRequest GetValidLoginRequest() => new()
    {
        Username = "testuser",
        Password = "Test123!"
    };
}