using ZeroTrustAPI.Api.Entities;

namespace ZeroTrustAPI.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user); // for testing/registration
}