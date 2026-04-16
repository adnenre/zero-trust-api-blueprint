using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZeroTrustAPI.Api.DTOs;

namespace ZeroTrustAPI.Api.Services.Interfaces;

public interface ISessionService
{
    Task<List<SessionDto>> GetSessionsForUserAsync(Guid userId);

    /// <summary>
    /// Revokes a session if it exists and belongs to the user.
    /// </summary>
    /// <returns>True if revoked, false if not found or not owned.</returns>
    Task<bool> RevokeSessionAsync(Guid sessionId, Guid userId);
}