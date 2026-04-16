using Microsoft.EntityFrameworkCore;
using Xunit;
using ZeroTrustAPI.Api.Data;
using ZeroTrustAPI.Api.Services.Implementations;
using ZeroTrustAPI.Api.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ZeroTrustAPI.Tests.Unit.Services;

public class SessionServiceTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetSessionsForUserAsync_ReturnsOnlyNonRevokedUserSessions()
    {
        await using var context = GetDbContext();
        var service = new SessionService(context);
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var session1 = new Session { Id = Guid.NewGuid(), UserId = userId1, CreatedAt = DateTime.UtcNow, IsRevoked = false };
        var session2 = new Session { Id = Guid.NewGuid(), UserId = userId2, CreatedAt = DateTime.UtcNow, IsRevoked = false };
        var session3 = new Session { Id = Guid.NewGuid(), UserId = userId1, CreatedAt = DateTime.UtcNow, IsRevoked = false };
        var session4 = new Session { Id = Guid.NewGuid(), UserId = userId1, CreatedAt = DateTime.UtcNow, IsRevoked = true }; // revoked, should be excluded
        context.Sessions.AddRange(session1, session2, session3, session4);
        await context.SaveChangesAsync();

        var sessions = await service.GetSessionsForUserAsync(userId1);

        Assert.Equal(2, sessions.Count);
        Assert.Contains(sessions, s => s.Id == session1.Id);
        Assert.Contains(sessions, s => s.Id == session3.Id);
        Assert.DoesNotContain(sessions, s => s.Id == session2.Id);
        Assert.DoesNotContain(sessions, s => s.Id == session4.Id);
    }

    [Fact]
    public async Task RevokeSessionAsync_ValidSession_RevokesItAndReturnsTrue()
    {
        await using var context = GetDbContext();
        var service = new SessionService(context);
        var userId = Guid.NewGuid();
        var session = new Session { Id = Guid.NewGuid(), UserId = userId, CreatedAt = DateTime.UtcNow, IsRevoked = false };
        context.Sessions.Add(session);
        await context.SaveChangesAsync();

        var result = await service.RevokeSessionAsync(session.Id, userId);

        Assert.True(result);
        var updated = await context.Sessions.FindAsync(session.Id);
        Assert.NotNull(updated);
        Assert.True(updated.IsRevoked);
    }

    [Fact]
    public async Task RevokeSessionAsync_WrongUserId_ReturnsFalseAndDoesNothing()
    {
        await using var context = GetDbContext();
        var service = new SessionService(context);
        var userId = Guid.NewGuid();
        var wrongUserId = Guid.NewGuid();
        var session = new Session { Id = Guid.NewGuid(), UserId = userId, CreatedAt = DateTime.UtcNow, IsRevoked = false };
        context.Sessions.Add(session);
        await context.SaveChangesAsync();

        var result = await service.RevokeSessionAsync(session.Id, wrongUserId);

        Assert.False(result);
        var updated = await context.Sessions.FindAsync(session.Id);
        Assert.NotNull(updated);
        Assert.False(updated.IsRevoked);
    }

    [Fact]
    public async Task RevokeSessionAsync_NonExistentSession_ReturnsFalseAndNoError()
    {
        await using var context = GetDbContext();
        var service = new SessionService(context);
        var userId = Guid.NewGuid();

        var result = await service.RevokeSessionAsync(Guid.NewGuid(), userId);

        Assert.False(result);
    }
}