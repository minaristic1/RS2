using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace GrizGo.E2ETests;

public class MenuItemManagementE2ETests
{
    [Fact]
    public async Task UpdateMenuItem_WithoutToken_ReturnsUnauthorized()
    {
        using var client = GatewayClient.Create();

        var response = await client.PutAsJsonAsync($"/api/menu-items/{Guid.NewGuid()}", new
        {
            nameSr = "x",
            nameEn = "x",
            descriptionSr = "",
            descriptionEn = "",
            price = 100,
            imageUrl = "",
            isAvailable = true,
            isFeatured = false,
            preparationTimeMinutes = 5
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMenuItem_WithoutToken_ReturnsUnauthorized()
    {
        using var client = GatewayClient.Create();

        var response = await client.DeleteAsync($"/api/menu-items/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateMenuItem_AsCustomer_ReturnsForbidden()
    {
        using var client = GatewayClient.Create();
        var token = await AuthHelper.RegisterAndLoginAsync(client, "Customer");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.PutAsJsonAsync($"/api/menu-items/{Guid.NewGuid()}", new
        {
            nameSr = "x",
            nameEn = "x",
            descriptionSr = "",
            descriptionEn = "",
            price = 100,
            imageUrl = "",
            isAvailable = true,
            isFeatured = false,
            preparationTimeMinutes = 5
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
