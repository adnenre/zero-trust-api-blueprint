using System;

namespace ZeroTrustAPI.Api.DTOs;

public class ApiKeyDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}