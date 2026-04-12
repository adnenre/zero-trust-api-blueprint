using Microsoft.EntityFrameworkCore;
using ZeroTrustAPI.Api.Data;
using ZeroTrustAPI.Api.Entities;
using ZeroTrustAPI.Api.Repositories.Implementations;
using Xunit;

namespace ZeroTrustAPI.Tests.Integration.Repositories;

public class UserRepositoryIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserRepository _repo;

    public UserRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _repo = new UserRepository(_context);
    }

    [Fact]
    public async Task AddAndGetByUsernameAsync_Integration_Works()
    {
        var user = new User { Username = "integrationUser", Email = "int@test.com", PasswordHash = "hash" };
        await _repo.AddAsync(user);
        var fetched = await _repo.GetByUsernameAsync("integrationUser");
        Assert.NotNull(fetched);
        Assert.Equal("int@test.com", fetched.Email);
    }

    public void Dispose() => _context.Dispose();
}