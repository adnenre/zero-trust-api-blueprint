using System.Threading.Tasks;
using ZeroTrustAPI.Api.Entities;
using ZeroTrustAPI.Api.DTOs;   // for PagedResult<T>

namespace ZeroTrustAPI.Api.Repositories.Interfaces;

/// <summary>
/// Defines the contract for audit log data access operations.
/// </summary>
public interface IAuditLogRepository
{
    /// <summary>
    /// Retrieves a paginated list of audit logs.
    /// </summary>
    /// <param name="page">Page number (1-indexed).</param>
    /// <param name="pageSize">Number of logs per page.</param>
    /// <returns>A <see cref="PagedResult{AuditLog}"/> containing the requested page and total count.</returns>
    Task<PagedResult<AuditLog>> GetPagedAsync(int page, int pageSize);
}