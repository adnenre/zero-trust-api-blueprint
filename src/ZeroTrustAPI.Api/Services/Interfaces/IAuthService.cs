using ZeroTrustAPI.Api.DTOs;

namespace ZeroTrustAPI.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(string username, string password);
}