using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZeroTrustAPI.Api.Data;
using ZeroTrustAPI.Api.DTOs;
using ZeroTrustAPI.Api.Entities;
using ZeroTrustAPI.Api.Services.Interfaces;

namespace ZeroTrustAPI.Api.Services.Implementations;

public class SessionService : ISessionService
{
    private readonly AppDbContext _context;

    public SessionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SessionDto>> GetSessionsForUserAsync(Guid userId)
    {
        var sessions = await _context.Sessions
            .Where(s => s.UserId == userId && !s.IsRevoked)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SessionDto
            {
                Id = s.Id,
                CreatedAt = s.CreatedAt,
                LastActivity = s.LastActivity,
                IpAddress = s.IpAddress
            })
            .ToListAsync();

        return sessions;
    }

    public async Task<bool> RevokeSessionAsync(Guid sessionId, Guid userId)
    {
        var session = await _context.Sessions
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId && !s.IsRevoked);

        if (session == null)
            return false;

        session.IsRevoked = true;
        await _context.SaveChangesAsync();
        return true;
    }
}