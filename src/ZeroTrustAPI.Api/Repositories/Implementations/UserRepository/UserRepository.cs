using Microsoft.EntityFrameworkCore;
using ZeroTrustAPI.Api.Data;
using ZeroTrustAPI.Api.Entities;
using ZeroTrustAPI.Api.Repositories.Interfaces;

namespace ZeroTrustAPI.Api.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}