using System;

namespace RestaurantService.Application.Security;

/// <summary>
/// Identity of the caller, extracted from the JWT claims, used to enforce
/// that a RestaurantOwner/RestaurantEmployee can only manage their own restaurant.
/// </summary>
public record RequestingUser(Guid Id, string Role, Guid? RestaurantId)
{
    public bool IsAdmin => Role == "Admin";

    public bool IsRestaurantOwner => Role == "RestaurantOwner";

    public bool IsRestaurantEmployee => Role == "RestaurantEmployee";

    /// <summary>
    /// True if this user is allowed to create/update/delete data belonging to
    /// the restaurant identified by <paramref name="restaurantId"/>, whose current
    /// owner is <paramref name="restaurantOwnerId"/> (may be null for legacy rows
    /// created before ownership tracking existed).
    /// </summary>
    public bool CanManageRestaurant(Guid restaurantId, Guid? restaurantOwnerId)
    {
        if (IsAdmin)
        {
            return true;
        }

        if (IsRestaurantOwner)
        {
            if (restaurantOwnerId.HasValue && restaurantOwnerId.Value == Id)
            {
                return true;
            }

            // Owner accounts created by Admin via /api/users/admin/staff are assigned
            // to a restaurant through the RestaurantId claim, not Restaurant.OwnerId
            // (that column belongs to a different microservice/database), so fall back
            // to the same claim-based check used for RestaurantEmployee.
            return RestaurantId.HasValue && RestaurantId.Value == restaurantId;
        }

        if (IsRestaurantEmployee)
        {
            return RestaurantId.HasValue && RestaurantId.Value == restaurantId;
        }

        return false;
    }
}
