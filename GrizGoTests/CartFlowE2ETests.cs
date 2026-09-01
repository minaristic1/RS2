using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace GrizGo.E2ETests;

public class CartFlowE2ETests
{
    [Fact]
    public async Task GetCart_ForNewUser_ReturnsEmptyCart()
    {
        using var client = GatewayClient.Create();
        var email = $"e2e-cart-{Guid.NewGuid():N}@grizgo.rs";
        const string password = "test12345";

        var registerResponse = await client.PostAsJsonAsync("/api/users/register", new
        {
            email,
            password,
            fullName = "E2E Cart Kupac",
            role = "Customer"
        });
        registerResponse.EnsureSuccessStatusCode();
        var registered = await registerResponse.Content.ReadFromJsonAsync<JsonElement>();
        var userId = registered.GetProperty("id").GetGuid();

        var loginResponse = await client.PostAsJsonAsync("/api/users/login", new { email, password });
        loginResponse.EnsureSuccessStatusCode();
        var login = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", login.GetProperty("token").GetString());

        var response = await client.GetAsync($"/api/carts/{userId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cart = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(0, cart.GetProperty("items").GetArrayLength());
    }

    [Fact]
    public async Task AddItem_WithNonExistentProduct_IsRejected()
    {
        using var client = GatewayClient.Create();
        var userId = Guid.NewGuid();

        var response = await client.PostAsJsonAsync($"/api/carts/{userId}/items", new
        {
            productId = Guid.NewGuid(),
            quantity = 1
        });

        Assert.False(response.IsSuccessStatusCode, "Dodavanje nepostojećeg proizvoda ne sme da uspe.");
    }
}
