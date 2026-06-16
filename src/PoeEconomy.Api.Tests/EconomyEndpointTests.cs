using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                Items: [
                    new("divine-orb", "Divine Orb"),
                    new("annulment-orb", "Orb of Annulment"),
                    new("chaos-orb", "Chaos Orb")
                ],
                Rates: new Dictionary<string, decimal> { ["exalted"] = 200m },
                Primary: "divine"
            ),
            Lines: [
                new("divine-orb", PrimaryValue: 5m),
                new("annulment-orb", PrimaryValue: 2m),
                new("chaos-orb", PrimaryValue: 0.3m)
            ]
        ));

        await using var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b
                .ConfigureTestServices(services =>
                    services.AddSingleton<IPoeNinjaClient>(fake))
                .ConfigureAppConfiguration((_, cfg) => {
                    cfg.Sources.Clear();
                    cfg.AddInMemoryCollection(new Dictionary<string, string?> {
                        ["Economy:League"] = "Test League",
                        ["Economy:ThresholdExalts"] = "100",
                        ["Economy:Sections:0:Type"] = "Currency",
                        ["Economy:Sections:0:Name"] = "Currency",
                        ["Economy:Sections:0:Group"] = "General"
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
                Items: [new("divine-orb", "Divine Orb")],
                Rates: new Dictionary<string, decimal> { ["exalted"] = 200m },
                Primary: "divine"
            ),
            Lines: [new("divine-orb", PrimaryValue: 5m)] // 1000 Ex
        ));
        fake.AddSection("Fragments", new PoeNinjaApiResponse(
            Core: new PoeNinjaCore(
                Items: [new("sacrifice-at-dawn", "Sacrifice at Dawn")],
                Rates: new Dictionary<string, decimal> { ["exalted"] = 200m },
                Primary: "divine"
            ),
            Lines: [new("sacrifice-at-dawn", PrimaryValue: 0.1m)] // 20 Ex — below threshold
        ));

        await using var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b
                .ConfigureTestServices(services =>
                    services.AddSingleton<IPoeNinjaClient>(fake))
                .ConfigureAppConfiguration((_, cfg) => {
                    cfg.Sources.Clear();
                    cfg.AddInMemoryCollection(new Dictionary<string, string?> {
                        ["Economy:League"] = "Test League",
                        ["Economy:ThresholdExalts"] = "500",
                        ["Economy:Sections:0:Type"] = "Currency",
                        ["Economy:Sections:0:Name"] = "Currency",
                        ["Economy:Sections:0:Group"] = "General",
                        ["Economy:Sections:1:Type"] = "Fragments",
                        ["Economy:Sections:1:Name"] = "Fragments",
                        ["Economy:Sections:1:Group"] = "General"
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
                Items: [new("divine-orb", "Divine Orb")],
                Rates: new Dictionary<string, decimal> { ["exalted"] = 189.5m },
                Primary: "divine"
            ),
            Lines: [new("divine-orb", PrimaryValue: 1m)]
        ));

        await using var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b
                .ConfigureTestServices(services =>
                    services.AddSingleton<IPoeNinjaClient>(fake))
                .ConfigureAppConfiguration((_, cfg) => {
                    cfg.Sources.Clear();
                    cfg.AddInMemoryCollection(new Dictionary<string, string?> {
                        ["Economy:League"] = "Test League",
                        ["Economy:ThresholdExalts"] = "0",
                        ["Economy:Sections:0:Type"] = "Currency",
                        ["Economy:Sections:0:Name"] = "Currency",
                        ["Economy:Sections:0:Group"] = "General"
                    });
                }));

        var client = app.CreateClient();
        var response = await client.GetFromJsonAsync<EconomyOverviewDto>("/api/economy");

        Assert.NotNull(response);
        Assert.Equal(189.5m, response.DivineOrbRate);
    }

    // poe.ninja should be called once per section (by the background cache service),
    // not once per HTTP request. Three requests → same 1 poe.ninja call.
    [Fact]
    public async Task PoeNinja_is_called_once_regardless_of_request_count()
    {
        var fake = new FakePoeNinjaClient();
        fake.AddSection("Currency", new PoeNinjaApiResponse(
            Core: new PoeNinjaCore(
                Items: [new("divine-orb", "Divine Orb")],
                Rates: new Dictionary<string, decimal> { ["exalted"] = 200m },
                Primary: "divine"
            ),
            Lines: [new("divine-orb", PrimaryValue: 1m)]
        ));

        await using var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b
                .ConfigureTestServices(services =>
                    services.AddSingleton<IPoeNinjaClient>(fake))
                .ConfigureAppConfiguration((_, cfg) => {
                    cfg.Sources.Clear();
                    cfg.AddInMemoryCollection(new Dictionary<string, string?> {
                        ["Economy:League"] = "Test League",
                        ["Economy:ThresholdExalts"] = "0",
                        ["Economy:Sections:0:Type"] = "Currency",
                        ["Economy:Sections:0:Name"] = "Currency",
                        ["Economy:Sections:0:Group"] = "General"
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
