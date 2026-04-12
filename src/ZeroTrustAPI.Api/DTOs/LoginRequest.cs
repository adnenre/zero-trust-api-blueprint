namespace ZeroTrustAPI.Api.DTOs;

public class LoginRequest
{
    public string Username { get; }
    public string Password { get; }
    public LoginRequest(string username, string password) =>
        (Username, Password) = (username, password);
}