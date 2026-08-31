using System.Text.Json.Serialization;

namespace RestaurantService.Domain.ValueObjects;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CuisineType
{
    Italijanska = 1,
    Srpska = 2,
    Kineska = 3,
    Meksicka = 4,
    Japanska = 5,
    FastFood = 6,
    Rostilj = 7,
    Zdrava = 8,
    Vegetarijanska = 9,
    Deserti = 10
}