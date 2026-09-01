using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace GrizGo.E2ETests;

public class RestaurantFlowE2ETests
{
    [Fact]
    public async Task CreateFindViewEditRestaurant_FullLifecycle()
    {
        using var client = GatewayClient.Create();
        var uniqueName = $"E2E Restoran {Guid.NewGuid():N}".Substring(0, 30);

        var ownerToken = await AuthHelper.CreateStaffAndLoginAsync(client, "RestaurantOwner");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ownerToken);

        var createResponse = await client.PostAsJsonAsync("/api/restaurants", new
        {
            nameSr = uniqueName,
            nameEn = uniqueName,
            descriptionSr = "Opis za E2E test",
            descriptionEn = "E2E test description",
            address = "Testna adresa 1",
            imageUrl = "https://picsum.photos/seed/e2etest/300/200",
            cuisineType = "Srpska",
            isActive = true,
            isFeatured = false
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var restaurantId = created.GetProperty("id").GetGuid();

        var searchResponse = await client.GetAsync($"/api/restaurants/search?term={Uri.EscapeDataString(uniqueName)}");
        Assert.Equal(HttpStatusCode.OK, searchResponse.StatusCode);
        var searchResults = await searchResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(searchResults.GetArrayLength() > 0, "Pretraga treba da nađe upravo kreirani restoran.");

        var detailResponse = await client.GetAsync($"/api/restaurants/{restaurantId}");
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
        var detail = await detailResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(uniqueName, detail.GetProperty("nameSr").GetString());

        var menuResponse = await client.GetAsync($"/api/restaurants/{restaurantId}/menu");
        Assert.Equal(HttpStatusCode.OK, menuResponse.StatusCode);

        var updatedName = uniqueName + " izmenjen";
        var updateResponse = await client.PutAsJsonAsync($"/api/restaurants/{restaurantId}", new
        {
            nameSr = updatedName,
            nameEn = updatedName,
            descriptionSr = "Izmenjen opis",
            descriptionEn = "Updated description",
            address = "Testna adresa 1",
            imageUrl = "https://picsum.photos/seed/e2etest/300/200",
            cuisineType = "Srpska",
            isActive = true,
            isFeatured = true
        });
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var afterUpdateResponse = await client.GetAsync($"/api/restaurants/{restaurantId}");
        var afterUpdate = await afterUpdateResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(updatedName, afterUpdate.GetProperty("nameSr").GetString());
        Assert.True(afterUpdate.GetProperty("isFeatured").GetBoolean());
    }

    [Fact]
    public async Task GetById_NonExistentRestaurant_ReturnsNotFound()
    {
        using var client = GatewayClient.Create();

        var response = await client.GetAsync($"/api/restaurants/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateRestaurant_WithoutToken_ReturnsUnauthorized()
    {
        using var client = GatewayClient.Create();

        var response = await client.PostAsJsonAsync("/api/restaurants", new
        {
            nameSr = "Neovlašćen restoran",
            nameEn = "Unauthorized restaurant",
            descriptionSr = "",
            descriptionEn = "",
            address = "x",
            imageUrl = "",
            cuisineType = "Srpska",
            isActive = true,
            isFeatured = false
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SetOpeningHours_AsOwner_Succeeds()
    {
        using var client = GatewayClient.Create();
        var token = await AuthHelper.CreateStaffAndLoginAsync(client, "RestaurantOwner");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createResponse = await client.PostAsJsonAsync("/api/restaurants", new
        {
            nameSr = $"E2E Radno Vreme {Guid.NewGuid():N}".Substring(0, 30),
            nameEn = "E2E Opening Hours",
            descriptionSr = "",
            descriptionEn = "",
            address = "Testna adresa 2",
            imageUrl = "https://picsum.photos/seed/e2ehours/300/200",
            cuisineType = "Srpska",
            isActive = true,
            isFeatured = false
        });
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var restaurantId = created.GetProperty("id").GetGuid();

        var hoursResponse = await client.PutAsJsonAsync($"/api/restaurants/{restaurantId}/opening-hours", new[]
        {
            new { dayOfWeek = 1, openTime = "08:00", closeTime = "22:00", isClosed = false },
            new { dayOfWeek = 2, openTime = "08:00", closeTime = "22:00", isClosed = false }
        });

        Assert.Equal(HttpStatusCode.NoContent, hoursResponse.StatusCode);
    }
}
