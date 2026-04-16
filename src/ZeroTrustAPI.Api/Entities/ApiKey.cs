using System;

namespace ZeroTrustAPI.Api.Entities;

public class ApiKey
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public User User { get; set; } = null!;
}