using System.Net.Http.Json;
using System.Text.Json;

namespace GrizGo.E2ETests;

public static class AuthHelper
{
    public static async Task<string> RegisterAndLoginAsync(HttpClient client, string role)
    {
        var email = $"e2e-{role.ToLower()}-{Guid.NewGuid():N}@grizgo.rs";
        const string password = "test12345";

        var registerResponse = await client.PostAsJsonAsync("/api/users/register", new
        {
            email,
            password,
            fullName = $"E2E {role}",
            role
        });
        registerResponse.EnsureSuccessStatusCode();

        return await LoginAsync(client, email, password);
    }

    public static Task<string> LoginAsAdminAsync(HttpClient client) =>
        LoginAsync(client, "admin@grizgo.rs", "AdminGrizGo2026!");

    public static async Task<string> CreateStaffAndLoginAsync(HttpClient client, string role, Guid? restaurantId = null)
    {
        var adminToken = await LoginAsAdminAsync(client);

        var email = $"e2e-{role.ToLower()}-{Guid.NewGuid():N}@grizgo.rs";
        const string password = "test12345";

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

        var createResponse = await client.PostAsJsonAsync("/api/users/admin/staff", new
        {
            email,
            password,
            fullName = $"E2E {role}",
            role,
            restaurantId
        });
        createResponse.EnsureSuccessStatusCode();

        return await LoginAsync(client, email, password);
    }

    private static async Task<string> LoginAsync(HttpClient client, string email, string password)
    {
        var loginResponse = await client.PostAsJsonAsync("/api/users/login", new { email, password });
        loginResponse.EnsureSuccessStatusCode();

        var body = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("token").GetString()!;
    }
}
