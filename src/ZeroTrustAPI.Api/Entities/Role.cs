using System;

namespace ZeroTrustAPI.Api.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}