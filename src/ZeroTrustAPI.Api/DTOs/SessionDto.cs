using System;

namespace ZeroTrustAPI.Api.DTOs;

public class SessionDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastActivity { get; set; }
    public string? IpAddress { get; set; }
}