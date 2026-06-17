using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PoeEconomy.Api;
using PoeEconomy.Api.PoeNinja;

namespace PoeEconomy.Api.Tests;

public class EconomyEndpointTests
{
    // Divine rate = 200 Exalts. Items: Divine (5 Divine = 1000 Ex), Annulment (2 Divine = 400 Ex), Chaos (0.3 Divine = 60 Ex).
    // Threshold = 100 Exalts → Divine and Annulment pass, Chaos does not.
    [Fact]
    public async Task Returns_items_above_threshold_sorted_by_value_descending()
    {
        var fake = new FakePoeNinjaClient();
        fake.AddSection("Currency", new PoeNinjaApiResponse(
            Core: new PoeNinjaCore(
                Items: [],
                Rates: new Dictionary<string, decimal> { ["exalted"] = 200m },
                Primary: "divine"
            ),
            Lines: [
                new("divine-orb", PrimaryValue: 5m),
                new("annulment-orb", PrimaryValue: 2m),
                new("chaos-orb", PrimaryValue: 0.3m)
            ],
            Items: [
                new("divine-orb", "Divine Orb"),
                new("annulment-orb", "Orb of Annulment"),
                new("chaos-orb", "Chaos Orb")
            ]
        ));

        var sections = new SectionStore();
        sections.Update([new PoeNinjaSection("Currency", "General", "Currency")]);

        await using var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton<IPoeNinjaClient>(fake);
                    services.AddSingleton(sections);
                })
                .ConfigureAppConfiguration((_, cfg) => {
                    cfg.Sources.Clear();
                    cfg.AddInMemoryCollection(new Dictionary<string, string?> {
                        ["Economy:League"] = "Test League",
                        ["Economy:ThresholdExalts"] = "100"
                    });
                }));

        var client = app.CreateClient();
        var response = await client.GetFromJsonAsync<EconomyOverviewDto>("/api/economy");

        Assert.NotNull(response);
        Assert.Single(response.Sections);

        var section = response.Sections[0];
        Assert.Equal("Currency", section.Name);
        Assert.Equal("General", section.Group);
        Assert.Equal(2, section.Items.Count);
        Assert.Equal("Divine Orb", section.Items[0].Name);
        Assert.Equal(1000m, section.Items[0].ValueInExalts);
        Assert.Equal("Orb of Annulment", section.Items[1].Name);
        Assert.Equal(400m, section.Items[1].ValueInExalts);
    }

    // Threshold = 500 Exalts. Currency section: Divine (1000 Ex) passes. Fragment section: all items below 500 Ex.
    // Expected: only Currency section appears in response.
    [Fact]
    public async Task Excludes_sections_where_no_items_exceed_threshold()
    {
        var fake = new FakePoeNinjaClient();
        fake.AddSection("Currency", new PoeNinjaApiResponse(
            Core: new PoeNinjaCore(
                Items: [],
                Rates: new Dictionary<string, decimal> { ["exalted"] = 200m },
                Primary: "divine"
            ),
            Lines: [new("divine-orb", PrimaryValue: 5m)], // 1000 Ex
            Items: [new("divine-orb", "Divine Orb")]
        ));
        fake.AddSection("Fragments", new PoeNinjaApiResponse(
            Core: new PoeNinjaCore(
                Items: [],
                Rates: new Dictionary<string, decimal> { ["exalted"] = 200m },
                Primary: "divine"
            ),
            Lines: [new("sacrifice-at-dawn", PrimaryValue: 0.1m)], // 20 Ex — below threshold
            Items: [new("sacrifice-at-dawn", "Sacrifice at Dawn")]
        ));

        var sections = new SectionStore();
        sections.Update([
            new PoeNinjaSection("Currency", "General", "Currency"),
            new PoeNinjaSection("Fragments", "General", "Fragments")
        ]);

        await using var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton<IPoeNinjaClient>(fake);
                    services.AddSingleton(sections);
                })
                .ConfigureAppConfiguration((_, cfg) => {
                    cfg.Sources.Clear();
                    cfg.AddInMemoryCollection(new Dictionary<string, string?> {
                        ["Economy:League"] = "Test League",
                        ["Economy:ThresholdExalts"] = "500"
                    });
                }));

        var client = app.CreateClient();
        var response = await client.GetFromJsonAsync<EconomyOverviewDto>("/api/economy");

        Assert.NotNull(response);
        Assert.Single(response.Sections);
        Assert.Equal("Currency", response.Sections[0].Name);
    }

    // The Divine Orb exchange rate must be present in the response so the frontend
    // can apply denomination-aware display (Exalts below 1 Divine, Divines at or above).
    [Fact]
    public async Task Response_includes_divine_orb_rate_from_section_data()
    {
        var fake = new FakePoeNinjaClient();
        fake.AddSection("Currency", new PoeNinjaApiResponse(
            Core: new PoeNinjaCore(
                Items: [],
                Rates: new Dictionary<string, decimal> { ["exalted"] = 189.5m },
                Primary: "divine"
            ),
            Lines: [new("divine-orb", PrimaryValue: 1m)],
            Items: [new("divine-orb", "Divine Orb")]
        ));

        var sections = new SectionStore();
        sections.Update([new PoeNinjaSection("Currency", "General", "Currency")]);

        await using var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton<IPoeNinjaClient>(fake);
                    services.AddSingleton(sections);
                })
                .ConfigureAppConfiguration((_, cfg) => {
                    cfg.Sources.Clear();
                    cfg.AddInMemoryCollection(new Dictionary<string, string?> {
                        ["Economy:League"] = "Test League",
                        ["Economy:ThresholdExalts"] = "0"
                    });
                }));

        var client = app.CreateClient();
        var response = await client.GetFromJsonAsync<EconomyOverviewDto>("/api/economy");

        Assert.NotNull(response);
        Assert.Equal(189.5m, response.DivineOrbRate);
    }

    // Real poe.ninja API: Core.Items has only 3 base currencies (divine, exalted, chaos).
    // All other items appear in the top-level Items list but not in Core.Items.
    // The cache service must look up names from top-level Items, not Core.Items.
    [Fact]
    public async Task Items_not_in_core_items_but_in_top_level_items_are_included()
    {
        var fake = new FakePoeNinjaClient();
        fake.AddSection("Currency", new PoeNinjaApiResponse(
            Core: new PoeNinjaCore(
                Items: [new("divine", "Divine Orb")],  // Core only has divine
                Rates: new Dictionary<string, decimal> { ["exalted"] = 200m },
                Primary: "divine"
            ),
            Lines: [
                new("divine", PrimaryValue: 1m),       // 200 Ex
                new("annulment", PrimaryValue: 1.5m)   // 300 Ex — in Lines but NOT in Core.Items
            ],
            Items: [
                new("divine", "Divine Orb"),
                new("annulment", "Orb of Annulment")   // present in top-level Items
            ]
        ));

        var sections = new SectionStore();
        sections.Update([new PoeNinjaSection("Currency", "General", "Currency")]);

        await using var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton<IPoeNinjaClient>(fake);
                    services.AddSingleton(sections);
                })
                .ConfigureAppConfiguration((_, cfg) => {
                    cfg.Sources.Clear();
                    cfg.AddInMemoryCollection(new Dictionary<string, string?> {
                        ["Economy:League"] = "Test League",
                        ["Economy:ThresholdExalts"] = "0"
                    });
                }));

        var client = app.CreateClient();
        var response = await client.GetFromJsonAsync<EconomyOverviewDto>("/api/economy");

        Assert.NotNull(response);
        var items = response.Sections[0].Items;
        Assert.Equal(2, items.Count);
        Assert.Equal("Orb of Annulment", items[0].Name); // 300 Ex — higher, so first
        Assert.Equal("Divine Orb", items[1].Name);
    }

    // poe.ninja should be called once per section (by the background cache service),
    // not once per HTTP request. Three requests → same 1 poe.ninja call.
    [Fact]
    public async Task PoeNinja_is_called_once_regardless_of_request_count()
    {
        var fake = new FakePoeNinjaClient();
        fake.AddSection("Currency", new PoeNinjaApiResponse(
            Core: new PoeNinjaCore(
                Items: [],
                Rates: new Dictionary<string, decimal> { ["exalted"] = 200m },
                Primary: "divine"
            ),
            Lines: [new("divine-orb", PrimaryValue: 1m)],
            Items: [new("divine-orb", "Divine Orb")]
        ));

        var sections = new SectionStore();
        sections.Update([new PoeNinjaSection("Currency", "General", "Currency")]);

        await using var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b
                .ConfigureTestServices(services =>
                {
                    services.AddSingleton<IPoeNinjaClient>(fake);
                    services.AddSingleton(sections);
                })
                .ConfigureAppConfiguration((_, cfg) => {
                    cfg.Sources.Clear();
                    cfg.AddInMemoryCollection(new Dictionary<string, string?> {
                        ["Economy:League"] = "Test League",
                        ["Economy:ThresholdExalts"] = "0"
                    });
                }));

        var client = app.CreateClient();
        await client.GetAsync("/api/economy");
        await client.GetAsync("/api/economy");
        await client.GetAsync("/api/economy");

        Assert.Equal(1, fake.CallCount);
    }
}

// DTOs mirroring the endpoint's JSON response shape
record EconomyOverviewDto(List<SectionDto> Sections, decimal DivineOrbRate);
record SectionDto(string Name, string Group, List<ItemDto> Items);
record ItemDto(string Name, decimal ValueInExalts);
