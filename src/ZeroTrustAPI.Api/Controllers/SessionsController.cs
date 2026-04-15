using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroTrustAPI.Api.Extensions;
using ZeroTrustAPI.Api.Services.Interfaces;

namespace ZeroTrustAPI.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/sessions")]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionsController(ISessionService sessionService) => _sessionService = sessionService;

    [HttpGet]
    public async Task<IActionResult> GetUserSessions()
    {
        var userId = User.GetUserId();
        var sessions = await _sessionService.GetSessionsForUserAsync(userId);
        return Ok(sessions);
    }

    [HttpDelete("{sessionId:guid}")]
    public async Task<IActionResult> RevokeSession(Guid sessionId)
    {
        var userId = User.GetUserId();
        var revoked = await _sessionService.RevokeSessionAsync(sessionId, userId);
        if (!revoked)
            return NotFound(); // Session does not exist or does not belong to the user

        return NoContent();
    }
}