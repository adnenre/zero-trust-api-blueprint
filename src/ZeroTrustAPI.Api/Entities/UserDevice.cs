using System;

namespace ZeroTrustAPI.Api.Entities;

public class Device
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public DateTime LastUsedAt { get; set; }
    public User User { get; set; } = null!;
}