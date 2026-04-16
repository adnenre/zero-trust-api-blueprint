using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroTrustAPI.Api.Repositories.Interfaces;

namespace ZeroTrustAPI.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/auditlogs")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogRepository _auditLogRepository;
    public AuditLogsController(IAuditLogRepository auditLogRepository) => _auditLogRepository = auditLogRepository;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var logs = await _auditLogRepository.GetPagedAsync(page, pageSize);
        return Ok(logs);
    }
}