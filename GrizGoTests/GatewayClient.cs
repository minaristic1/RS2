using System.Net.Http.Headers;

namespace GrizGo.E2ETests;

public static class GatewayClient
{
    public const string BaseUrl = "http://localhost:5029";

    public static HttpClient Create()
    {
        var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        client.DefaultRequestHeaders.Add("ClientId", "e2e-tests");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }
}
