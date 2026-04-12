using Microsoft.AspNetCore.Mvc;
using ZeroTrustAPI.Api.DTOs;
using ZeroTrustAPI.Api.Services.Interfaces;

namespace ZeroTrustAPI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.AuthenticateAsync(request.Username, request.Password);
        return result.Success ? Ok(result) : Unauthorized(result);
    }
}