using Microsoft.Extensions.Options;
using PoeEconomy.Api.Options;
using PoeEconomy.Api.PoeNinja;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EconomyOptions>(
    builder.Configuration.GetSection(EconomyOptions.SectionName));
builder.Services.AddHttpClient<IPoeNinjaClient, PoeNinjaHttpClient>();

var app = builder.Build();

app.MapGet("/api/economy", async (
    IPoeNinjaClient poeNinja,
    IOptions<EconomyOptions> options) =>
{
    var opts = options.Value;
    var sections = new List<object>();
    decimal divineRate = 0;

    foreach (var sectionCfg in opts.Sections)
    {
        var apiResponse = await poeNinja.GetSectionAsync(opts.League, sectionCfg.Type);

        if (divineRate == 0 && apiResponse.Core.Rates.TryGetValue("exalted", out var rate))
            divineRate = rate;

        var nameById = apiResponse.Core.Items.ToDictionary(x => x.Id, x => x.Name);

        var items = apiResponse.Lines
            .Where(l => nameById.ContainsKey(l.Id))
            .Select(l => new
            {
                Name = nameById[l.Id],
                ValueInExalts = l.PrimaryValue * divineRate
            })
            .Where(i => i.ValueInExalts >= opts.ThresholdExalts)
            .OrderByDescending(i => i.ValueInExalts)
            .ToList();

        if (items.Count > 0)
            sections.Add(new { sectionCfg.Name, sectionCfg.Group, Items = items });
    }

    return new { Sections = sections, DivineOrbRate = divineRate };
});

app.Run();

public partial class Program { }
