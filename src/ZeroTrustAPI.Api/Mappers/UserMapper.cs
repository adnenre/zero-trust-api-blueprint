using ZeroTrustAPI.Api.DTOs;
using ZeroTrustAPI.Api.Entities;

namespace ZeroTrustAPI.Api.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(User user) => new UserDto(user.Id, user.Username);
}