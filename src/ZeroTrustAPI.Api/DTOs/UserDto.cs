namespace ZeroTrustAPI.Api.DTOs;

public class UserDto
{
    public int Id { get; }
    public string Username { get; }
    public UserDto(int id, string username) => (Id, Username) = (id, username);
}