using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroTrustAPI.Api.DTOs;
using ZeroTrustAPI.Api.Services.Interfaces;
using ZeroTrustAPI.Api.Extensions;

namespace ZeroTrustAPI.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result.Success)
            return BadRequest(result.Errors);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (!result.Success)
            return Unauthorized(result.Errors);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);
        if (!result.Success)
            return Unauthorized(result.Errors);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.GetUserId();
        await _authService.RevokeRefreshTokensAsync(userId);
        return NoContent();
    }
}