using System.Net.Http.Json;

namespace PoeEconomy.Api.PoeNinja;

public class PoeNinjaHttpClient(HttpClient httpClient) : IPoeNinjaClient
{
    private const string ApiBase = "https://poe.ninja/poe2/api/economy/exchange/current/overview";

    public async Task<PoeNinjaApiResponse> GetSectionAsync(string league, string sectionType)
    {
        var url = $"{ApiBase}?league={Uri.EscapeDataString(league)}&type={sectionType}";
        return await httpClient.GetFromJsonAsync<PoeNinjaApiResponse>(url, JsonOptions.Default)
               ?? throw new InvalidOperationException($"Empty response from poe.ninja for section '{sectionType}'");
    }
}

file static class JsonOptions
{
    public static readonly System.Text.Json.JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
    };
}
