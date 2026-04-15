using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZeroTrustAPI.Api.DTOs;
using ZeroTrustAPI.Api.Entities;

namespace ZeroTrustAPI.Api.Services.Interfaces;

public interface IApiKeyService
{
    Task<ApiKey> CreateAsync(Guid userId);
    Task<IEnumerable<ApiKeyDto>> GetUserKeysAsync(Guid userId);
}