using Microsoft.EntityFrameworkCore;
using ZeroTrustAPI.Api.Data;
using ZeroTrustAPI.Api.Entities;
using ZeroTrustAPI.Api.Repositories.Interfaces;
using ZeroTrustAPI.Api.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace ZeroTrustAPI.Api.Repositories.Implementations;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AuditLog>> GetPagedAsync(int page, int pageSize)
    {
        var query = _context.AuditLogs.AsNoTracking();
        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(log => log.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<AuditLog>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}