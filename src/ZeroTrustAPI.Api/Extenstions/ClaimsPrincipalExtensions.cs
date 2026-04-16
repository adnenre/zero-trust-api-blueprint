using System;
using System.Security.Claims;

namespace ZeroTrustAPI.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("sub");
        if (claim == null) throw new InvalidOperationException("User ID claim not found.");
        return Guid.Parse(claim.Value);
    }
}