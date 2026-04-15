using System;

namespace ZeroTrustAPI.Api.Entities;

public class Session
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastActivity { get; set; }
    public string? IpAddress { get; set; }
    public bool IsRevoked { get; set; }
}