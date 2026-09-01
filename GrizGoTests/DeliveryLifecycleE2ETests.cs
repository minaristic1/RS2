using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace GrizGo.E2ETests;

public class DeliveryLifecycleE2ETests
{
    private static object BuildCreateRequest(Guid orderId) => new
    {
        orderId,
        customerName = "E2E Kupac",
        customerPhone = "0601234567",
        restaurantId = Guid.NewGuid(),
        restaurantName = "E2E Test Restoran",
        pickupAddress = "Adresa restorana 1",
        deliveryAddress = "Adresa dostave 2",
        totalPrice = 500,
        items = new[]
        {
            new { productName = "Test stavka", quantity = 2, unitPrice = 250 }
        }
    };

    [Fact]
    public async Task FullDeliveryLifecycle_RestaurantConfirmsThenCourierDelivers()
    {
        using var client = GatewayClient.Create();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await AuthHelper.LoginAsAdminAsync(client));
        var orderId = Guid.NewGuid();

        var createResponse = await client.PostAsJsonAsync("/api/delivery", BuildCreateRequest(orderId));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var deliveryId = created.GetProperty("id").GetGuid();
        Assert.Equal(0, created.GetProperty("status").GetInt32());

        var byOrderResponse = await client.GetAsync($"/api/delivery/by-order/{orderId}");
        Assert.Equal(HttpStatusCode.OK, byOrderResponse.StatusCode);

        var confirmResponse = await client.PostAsync($"/api/delivery/{deliveryId}/advance-status", null);
        Assert.Equal(HttpStatusCode.OK, confirmResponse.StatusCode);
        var confirmed = await confirmResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(1, confirmed.GetProperty("status").GetInt32());

        var courierId = Guid.NewGuid();
        var assignResponse = await client.PostAsync($"/api/delivery/{deliveryId}/assign-courier?courierId={courierId}", null);
        Assert.Equal(HttpStatusCode.OK, assignResponse.StatusCode);
        var assigned = await assignResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(courierId, assigned.GetProperty("courierId").GetGuid());
        Assert.Equal(1, assigned.GetProperty("status").GetInt32());

        var preparingResponse = await client.PostAsync($"/api/delivery/{deliveryId}/advance-status", null);
        var preparing = await preparingResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(2, preparing.GetProperty("status").GetInt32());

        var readyResponse = await client.PostAsync($"/api/delivery/{deliveryId}/advance-status", null);
        var ready = await readyResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(3, ready.GetProperty("status").GetInt32());

        var outForDeliveryResponse = await client.PostAsync($"/api/delivery/{deliveryId}/advance-status", null);
        var outForDelivery = await outForDeliveryResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(4, outForDelivery.GetProperty("status").GetInt32());

        var deliveredResponse = await client.PostAsync($"/api/delivery/{deliveryId}/advance-status", null);
        Assert.Equal(HttpStatusCode.OK, deliveredResponse.StatusCode);
        var delivered = await deliveredResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(5, delivered.GetProperty("status").GetInt32());
        Assert.False(string.IsNullOrEmpty(delivered.GetProperty("deliveredAt").GetString()));

        var afterDeliveredResponse = await client.PostAsync($"/api/delivery/{deliveryId}/advance-status", null);
        Assert.Equal(HttpStatusCode.BadRequest, afterDeliveredResponse.StatusCode);
    }

    [Fact]
    public async Task CancelDelivery_BeforeDelivered_SetsCancelledStatus()
    {
        using var client = GatewayClient.Create();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", await AuthHelper.LoginAsAdminAsync(client));
        var orderId = Guid.NewGuid();

        var createResponse = await client.PostAsJsonAsync("/api/delivery", BuildCreateRequest(orderId));
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var deliveryId = created.GetProperty("id").GetGuid();

        var cancelResponse = await client.PostAsync($"/api/delivery/{deliveryId}/cancel", null);

        Assert.Equal(HttpStatusCode.OK, cancelResponse.StatusCode);
        var cancelled = await cancelResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(6, cancelled.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task GetByOrderId_NonExistentOrder_ReturnsNotFound()
    {
        using var client = GatewayClient.Create();

        var response = await client.GetAsync($"/api/delivery/by-order/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
