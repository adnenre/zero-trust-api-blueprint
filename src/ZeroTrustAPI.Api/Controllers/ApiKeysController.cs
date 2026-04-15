using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroTrustAPI.Api.Extensions;
using ZeroTrustAPI.Api.Services.Interfaces;

namespace ZeroTrustAPI.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/apikeys")]
public class ApiKeysController : ControllerBase
{
    private readonly IApiKeyService _apiKeyService;
    public ApiKeysController(IApiKeyService apiKeyService) => _apiKeyService = apiKeyService;

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var userId = User.GetUserId();
        var apiKey = await _apiKeyService.CreateAsync(userId);
        return Ok(new { apiKey.Key });
    }

    [HttpGet]
    public async Task<IActionResult> GetUserKeys()
    {
        var userId = User.GetUserId();
        var keys = await _apiKeyService.GetUserKeysAsync(userId);
        return Ok(keys);
    }
}