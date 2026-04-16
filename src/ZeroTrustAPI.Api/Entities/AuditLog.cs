using System;

namespace ZeroTrustAPI.Api.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
}