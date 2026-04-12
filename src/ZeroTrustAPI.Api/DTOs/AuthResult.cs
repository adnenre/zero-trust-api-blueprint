using ZeroTrustAPI.Api.DTOs;

namespace ZeroTrustAPI.Api.DTOs;

public class AuthResult
{
    public bool Success { get; }
    public UserDto? User { get; }
    public string Message { get; }
    public AuthResult(bool success, UserDto? user, string message) =>
        (Success, User, Message) = (success, user, message);
}