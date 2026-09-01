using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace GrizGo.E2ETests;

public class BillingFlowE2ETests
{
    private const int PollAttempts = 15;
    private static readonly TimeSpan PollDelay = TimeSpan.FromSeconds(1);

    [Fact]
    public async Task Checkout_Payment_CreatesDeliveryAutomatically()
    {
        using var client = GatewayClient.Create();

        var ownerToken = await AuthHelper.CreateStaffAndLoginAsync(client, "RestaurantOwner");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ownerToken);

        var restaurantResponse = await client.PostAsJsonAsync("/api/restaurants", new
        {
            nameSr = $"E2E Billing Restoran {Guid.NewGuid():N}".Substring(0, 30),
            nameEn = "E2E Billing Restaurant",
            descriptionSr = "",
            descriptionEn = "",
            address = "Testna adresa za placanje 1",
            imageUrl = "https://picsum.photos/seed/e2ebilling/300/200",
            cuisineType = "Srpska",
            isActive = true,
            isFeatured = false
        });
        restaurantResponse.EnsureSuccessStatusCode();
        var restaurant = await restaurantResponse.Content.ReadFromJsonAsync<JsonElement>();
        var restaurantId = restaurant.GetProperty("id").GetGuid();

        var menuResponse = await client.PostAsJsonAsync($"/api/restaurants/{restaurantId}/menus", new
        {
            nameSr = "Glavni meni",
            nameEn = "Main menu",
            descriptionSr = "",
            descriptionEn = "",
            displayOrder = 1
        });
        menuResponse.EnsureSuccessStatusCode();
        var menu = await menuResponse.Content.ReadFromJsonAsync<JsonElement>();
        var menuId = menu.GetProperty("id").GetGuid();

        var categoryResponse = await client.PostAsJsonAsync($"/api/restaurants/{restaurantId}/menus/{menuId}/categories", new
        {
            nameSr = "Test kategorija",
            nameEn = "Test category",
            descriptionSr = "",
            descriptionEn = "",
            displayOrder = 1
        });
        categoryResponse.EnsureSuccessStatusCode();
        var category = await categoryResponse.Content.ReadFromJsonAsync<JsonElement>();
        var categoryId = category.GetProperty("id").GetGuid();

        var itemResponse = await client.PostAsJsonAsync(
            $"/api/restaurants/{restaurantId}/menus/{menuId}/categories/{categoryId}/items",
            new
            {
                nameSr = "Test jelo",
                nameEn = "Test dish",
                descriptionSr = "",
                descriptionEn = "",
                price = 300,
                imageUrl = "https://picsum.photos/seed/e2ebillingitem/300/200",
                isAvailable = true,
                isFeatured = false,
                preparationTimeMinutes = 10
            });
        itemResponse.EnsureSuccessStatusCode();
        var item = await itemResponse.Content.ReadFromJsonAsync<JsonElement>();
        var itemId = item.GetProperty("id").GetGuid();

        var customerRegisterResponse = await client.PostAsJsonAsync("/api/users/register", new
        {
            email = $"e2e-billing-kupac-{Guid.NewGuid():N}@grizgo.rs",
            password = "test12345",
            fullName = "E2E Billing Kupac",
            role = "Customer"
        });
        customerRegisterResponse.EnsureSuccessStatusCode();
        var customer = await customerRegisterResponse.Content.ReadFromJsonAsync<JsonElement>();
        var customerId = customer.GetProperty("id").GetGuid();

        var customerLoginResponse = await client.PostAsJsonAsync("/api/users/login", new
        {
            email = customer.GetProperty("email").GetString(),
            password = "test12345"
        });
        customerLoginResponse.EnsureSuccessStatusCode();
        var customerLogin = await customerLoginResponse.Content.ReadFromJsonAsync<JsonElement>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", customerLogin.GetProperty("token").GetString());

        var addItemResponse = await client.PostAsJsonAsync($"/api/carts/{customerId}/items", new
        {
            productId = itemId,
            quantity = 1
        });
        addItemResponse.EnsureSuccessStatusCode();

        var checkoutResponse = await client.PostAsJsonAsync($"/api/carts/{customerId}/checkout", new
        {
            deliveryAddress = "E2E test adresa dostave 42"
        });
        Assert.Equal(HttpStatusCode.Accepted, checkoutResponse.StatusCode);

        JsonElement invoice = default;
        var invoiceFound = false;
        for (var i = 0; i < PollAttempts && !invoiceFound; i++)
        {
            await Task.Delay(PollDelay);
            var invoicesResponse = await client.GetAsync($"/api/invoices/customer/{customerId}");
            invoicesResponse.EnsureSuccessStatusCode();
            var invoices = await invoicesResponse.Content.ReadFromJsonAsync<JsonElement>();
            foreach (var candidate in invoices.EnumerateArray())
            {
                if (candidate.GetProperty("status").GetString() == "AwaitingPayment")
                {
                    invoice = candidate;
                    invoiceFound = true;
                    break;
                }
            }
        }
        Assert.True(invoiceFound, "Billing nije napravio racun za porudzbinu u ocekivanom roku.");

        var invoiceId = invoice.GetProperty("id").GetGuid();
        var orderId = invoice.GetProperty("orderId").GetGuid();
        Assert.Equal(300, invoice.GetProperty("totalAmount").GetDecimal());

        var payResponse = await client.PostAsJsonAsync($"/api/invoices/{invoiceId}/payments", new
        {
            method = 1,
            provider = "E2E Simulacija",
            transactionReference = Guid.NewGuid().ToString()
        });
        Assert.Equal(HttpStatusCode.OK, payResponse.StatusCode);

        JsonElement delivery = default;
        var deliveryFound = false;
        for (var i = 0; i < PollAttempts && !deliveryFound; i++)
        {
            await Task.Delay(PollDelay);
            var deliveryResponse = await client.GetAsync($"/api/delivery/by-order/{orderId}");
            if (deliveryResponse.StatusCode == HttpStatusCode.OK)
            {
                delivery = await deliveryResponse.Content.ReadFromJsonAsync<JsonElement>();
                deliveryFound = true;
            }
        }
        Assert.True(deliveryFound, "Delivery nije napravio dostavu za placenu porudzbinu u ocekivanom roku.");

        Assert.Equal(orderId, delivery.GetProperty("orderId").GetGuid());
        Assert.Equal("E2E test adresa dostave 42", delivery.GetProperty("deliveryAddress").GetString());
        Assert.Equal(0, delivery.GetProperty("status").GetInt32());
    }
}
