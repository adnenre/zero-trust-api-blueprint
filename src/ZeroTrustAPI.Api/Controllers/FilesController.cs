using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroTrustAPI.Api.Extensions;
using ZeroTrustAPI.Api.Services.Interfaces;

namespace ZeroTrustAPI.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly IFileUploadService _fileService;
    public FilesController(IFileUploadService fileService) => _fileService = fileService;

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");
        var userId = User.GetUserId();
        var result = await _fileService.UploadAsync(file, userId);
        return Ok(new { result.Id, result.FileName });
    }
}