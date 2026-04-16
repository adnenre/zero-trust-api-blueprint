using Microsoft.EntityFrameworkCore;
using Xunit;
using ZeroTrustAPI.Api.Data;
using ZeroTrustAPI.Api.Services.Implementations;
using ZeroTrustAPI.Api.Entities;

namespace ZeroTrustAPI.Tests.Unit.Services;

public class ApiKeyServiceTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ReturnsValidApiKey()
    {
        await using var context = GetDbContext();
        var service = new ApiKeyService(context);
        var userId = Guid.NewGuid();

        var key = await service.CreateAsync(userId);

        Assert.NotNull(key.Key);
        Assert.True(key.IsActive);
        Assert.Equal(userId, key.UserId);
        Assert.Equal(DateTime.UtcNow.Date, key.CreatedAt.Date);

        var saved = await context.ApiKeys.FindAsync(key.Id);
        Assert.NotNull(saved);
        Assert.Equal(key.Key, saved.Key);
    }

    [Fact]
    public async Task GetUserKeysAsync_ReturnsOnlyUserKeys()
    {
        await using var context = GetDbContext();
        var service = new ApiKeyService(context);
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var key1 = await service.CreateAsync(userId1);
        var key2 = await service.CreateAsync(userId2);
        var key3 = await service.CreateAsync(userId1);

        var keysForUser1 = await service.GetUserKeysAsync(userId1);
        Assert.Equal(2, keysForUser1.Count());
        Assert.Contains(keysForUser1, k => k.Id == key1.Id);
        Assert.Contains(keysForUser1, k => k.Id == key3.Id);
        Assert.DoesNotContain(keysForUser1, k => k.Id == key2.Id);
    }

    [Fact]
    public async Task GetUserKeysAsync_EmptyIfNoKeys()
    {
        await using var context = GetDbContext();
        var service = new ApiKeyService(context);
        var userId = Guid.NewGuid();

        var keys = await service.GetUserKeysAsync(userId);
        Assert.Empty(keys);
    }
}