using ZeroTrustAPI.Api.DTOs;
using ZeroTrustAPI.Api.Services.Interfaces;
using ZeroTrustAPI.Api.Repositories.Interfaces;
using ZeroTrustAPI.Api.Security;
using ZeroTrustAPI.Api.Mappers;

namespace ZeroTrustAPI.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResult> AuthenticateAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            return new AuthResult(false, null, "User not found");

        if (!_passwordHasher.Verify(password, user.PasswordHash))
            return new AuthResult(false, null, "Invalid password");

        var userDto = UserMapper.ToDto(user);
        return new AuthResult(true, userDto, "Login successful");
    }
}