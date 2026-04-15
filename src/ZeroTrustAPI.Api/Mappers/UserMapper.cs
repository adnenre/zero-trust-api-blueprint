using ZeroTrustAPI.Api.DTOs;
using ZeroTrustAPI.Api.Entities;

namespace ZeroTrustAPI.Api.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(User user)
    {
        if (user == null)
            return null!;

        return new UserDto
        {
            Id = user.Id,           // Guid → Guid (no conversion error)
            Email = user.Email,
            // Add other properties as needed
        };
    }

    public static User ToEntity(UserDto dto)
    {
        if (dto == null)
            return null!;

        return new User
        {
            Id = dto.Id,
            Email = dto.Email,
            // Add other properties as needed
        };
    }

    // If you have a method that previously took an int, change the parameter type to Guid
    public static UserDto GetById(Guid id)   // was 'int id'
    {
        // Example: retrieve from repository (not implemented here)
        throw new NotImplementedException();
    }
}