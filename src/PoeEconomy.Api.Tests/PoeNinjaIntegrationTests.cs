using PoeEconomy.Api.PoeNinja;

namespace PoeEconomy.Api.Tests;

public class PoeNinjaHtmlParserTests
{
    // Copied from the real poe.ninja nav HTML (Runes of Aldur, 2026-06-17)
    private const string SampleHtml = """
        <div>
          <h3 data-variant="subdued">General</h3>
          <ul>
            <li><a href="/poe2/economy/runesofaldur/currency" title="Currency"><span>Currency</span></a></li>
            <li><a href="/poe2/economy/runesofaldur/abyssal-bones" title="Abyssal Bones"><span>Abyssal Bones</span></a></li>
            <li><a href="/poe2/economy/runesofaldur/soul-cores" title="Soul Cores"><span>Soul Cores</span></a></li>
          </ul>
          <h3 data-variant="subdued">Equipment</h3>
          <ul>
            <li><a href="/poe2/economy/runesofaldur/unique-weapons" title="Unique Weapons"><span>Unique Weapons</span></a></li>
          </ul>
          <h3 data-variant="subdued">Atlas</h3>
          <ul>
            <li><a href="/poe2/economy/runesofaldur/precursor-tablets" title="Precursor Tablets"><span>Precursor Tablets</span></a></li>
          </ul>
        </div>
        """;

    [Fact]
    public void ParseNavSections_extracts_groups_and_names()
    {
        var sections = PoeNinjaHtmlParser.ParseNavSections(SampleHtml);

        Assert.Equal(5, sections.Count);
        Assert.Equal("Currency",          sections[0].Name);
        Assert.Equal("General",           sections[0].Group);
        Assert.Equal("Currency",          sections[0].Type);
        Assert.Equal("Abyssal Bones",     sections[1].Name);
        Assert.Equal("General",           sections[1].Group);
        Assert.Equal("AbyssalBones",      sections[1].Type);
        Assert.Equal("Soul Cores",        sections[2].Name);
        Assert.Equal("SoulCores",         sections[2].Type);
        Assert.Equal("Unique Weapons",    sections[3].Name);
        Assert.Equal("Equipment",         sections[3].Group);
        Assert.Equal("UniqueWeapons",     sections[3].Type);
        Assert.Equal("Precursor Tablets", sections[4].Name);
        Assert.Equal("Atlas",             sections[4].Group);
        Assert.Equal("PrecursorTablets",  sections[4].Type);
    }

    [Fact]
    public void ParseNavSections_returns_empty_when_no_groups_found()
    {
        var sections = PoeNinjaHtmlParser.ParseNavSections("<html><body>no nav here</body></html>");
        Assert.Empty(sections);
    }
}

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
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; PoeEconomy/1.0)");
        var client = new PoeNinjaHttpClient(httpClient);

        var response = await client.GetSectionAsync(League, "Currency");

        Assert.NotNull(response);
        Assert.NotEmpty(response.Lines);
        Assert.NotEmpty(response.Core.Items);
        Assert.True(response.Core.Rates.ContainsKey("exalted"), "Expected 'exalted' key in Core.Rates");
        Assert.True(response.Core.Rates["exalted"] > 0);
        Assert.Equal("divine", response.Core.Primary);
    }
}
