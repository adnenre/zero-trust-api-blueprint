using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZeroTrustAPI.Api.Entities;

namespace ZeroTrustAPI.Api.Repositories.Interfaces;

public interface IApiKeyRepository
{
    Task<ApiKey?> GetByIdAsync(Guid id);
    Task<IEnumerable<ApiKey>> GetByUserIdAsync(Guid userId);
    Task AddAsync(ApiKey apiKey);
    Task UpdateAsync(ApiKey apiKey);
}