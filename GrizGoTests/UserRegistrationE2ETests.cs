using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace GrizGo.E2ETests;

public class UserRegistrationE2ETests
{
    [Fact]
    public async Task Register_NewUser_ReturnsCreatedWithUserData()
    {
        using var client = GatewayClient.Create();
        var email = $"e2e-{Guid.NewGuid():N}@grizgo.rs";

        var response = await client.PostAsJsonAsync("/api/users/register", new
        {
            email,
            password = "test12345",
            fullName = "E2E Test Korisnik",
            role = "Customer"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(email, body.GetProperty("email").GetString());
        Assert.Equal("Customer", body.GetProperty("role").GetString());
        Assert.NotEqual(Guid.Empty, body.GetProperty("id").GetGuid());
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        using var client = GatewayClient.Create();
        var email = $"e2e-dup-{Guid.NewGuid():N}@grizgo.rs";
        var request = new
        {
            email,
            password = "test12345",
            fullName = "Duplikat",
            role = "Customer"
        };

        var first = await client.PostAsJsonAsync("/api/users/register", request);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await client.PostAsJsonAsync("/api/users/register", request);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }
}
