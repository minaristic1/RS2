using System;
using System.Security.Claims;

using RestaurantService.Application.Security;

namespace RestaurantService.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Builds a <see cref="RequestingUser"/> from the JWT claims of the current caller,
    /// so application services can enforce "only your own restaurant" checks.
    /// </summary>
    public static RequestingUser ToRequestingUser(this ClaimsPrincipal principal)
    {
        var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("Token je bez NameIdentifier claim-a.");

        var role = principal.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        var restaurantIdClaim = principal.FindFirst("restaurantId")?.Value;
        Guid? restaurantId = restaurantIdClaim is null ? null : Guid.Parse(restaurantIdClaim);

        return new RequestingUser(Guid.Parse(idClaim), role, restaurantId);
    }
}
