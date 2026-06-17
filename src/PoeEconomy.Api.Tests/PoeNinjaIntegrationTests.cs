using PoeEconomy.Api.PoeNinja;

namespace PoeEconomy.Api.Tests;

/// <summary>
/// Hits the real poe.ninja API. Run with: dotnet test --filter "Category=Integration"
/// Skip in CI with: dotnet test --filter "Category!=Integration"
/// </summary>
[Trait("Category", "Integration")]
public class PoeNinjaIntegrationTests
{
    private const string League = "Runes of Aldur";

    [Fact]
    public async Task GetSectionAsync_Currency_returns_real_data()
    {
        using var httpClient = new HttpClient();
        var client = new PoeNinjaHttpClient(httpClient);

        var response = await client.GetSectionAsync(League, "Currency");

        Assert.NotNull(response);
        Assert.NotEmpty(response.Lines);
        Assert.NotEmpty(response.Core.Items);
        Assert.True(response.Core.Rates.ContainsKey("exalted"), "Expected 'exalted' key in Core.Rates");
        Assert.True(response.Core.Rates["exalted"] > 0, "Exalted rate should be positive");
        Assert.Equal("divine", response.Core.Primary);
    }

    [Theory]
    [InlineData("Fragments")]
    [InlineData("Scarabs")]
    [InlineData("Runes")]
    [InlineData("Essences")]
    [InlineData("Tablets")]
    [InlineData("Maps")]
    public async Task GetSectionAsync_configured_section_types_return_non_empty_response(string sectionType)
    {
        using var httpClient = new HttpClient();
        var client = new PoeNinjaHttpClient(httpClient);

        var response = await client.GetSectionAsync(League, sectionType);

        Assert.NotNull(response);
        Assert.NotEmpty(response.Lines);
    }
}
