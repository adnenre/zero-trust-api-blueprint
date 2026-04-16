using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZeroTrustAPI.Api.Data;
using ZeroTrustAPI.Api.DTOs;
using ZeroTrustAPI.Api.Entities;
using ZeroTrustAPI.Api.Services.Interfaces;

namespace ZeroTrustAPI.Api.Services.Implementations;

public class ApiKeyService : IApiKeyService
{
    private readonly AppDbContext _context;
    public ApiKeyService(AppDbContext context) => _context = context;

    public async Task<ApiKey> CreateAsync(Guid userId)
    {
        var key = new ApiKey
        {
            Id = Guid.NewGuid(),
            Key = GenerateApiKey(),
            UserId = userId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.ApiKeys.Add(key);
        await _context.SaveChangesAsync();
        return key;
    }

    public async Task<IEnumerable<ApiKeyDto>> GetUserKeysAsync(Guid userId)
    {
        var keys = await _context.ApiKeys
            .Where(k => k.UserId == userId)
            .Select(k => new ApiKeyDto
            {
                Id = k.Id,
                Key = k.Key,
                IsActive = k.IsActive,
                CreatedAt = k.CreatedAt
            })
            .ToListAsync();
        return keys;
    }

    private string GenerateApiKey()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}