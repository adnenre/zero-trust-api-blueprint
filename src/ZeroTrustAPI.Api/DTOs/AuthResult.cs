using System;
using System.Collections.Generic;

namespace ZeroTrustAPI.Api.DTOs;

public class AuthResult
{
    public bool Success { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public Guid? UserId { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}