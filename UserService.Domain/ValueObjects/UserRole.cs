using System.Text.Json.Serialization;

namespace UserService.Domain.ValueObjects
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        Customer = 1,
        RestaurantOwner = 2,
        RestaurantEmployee = 3,
        Driver = 4,
        Admin = 5
    }
}