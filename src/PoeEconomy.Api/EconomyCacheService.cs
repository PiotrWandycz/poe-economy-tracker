using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PoeEconomy.Api.Options;
using PoeEconomy.Api.PoeNinja;

namespace PoeEconomy.Api;

public class EconomyCacheService(
    IPoeNinjaClient poeNinja,
    IMemoryCache cache,
    IOptions<EconomyOptions> options,
    SectionStore sectionStore) : BackgroundService
{
    public const string CacheKey = "economy:overview";

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await RefreshAsync(cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            await RefreshAsync(stoppingToken);
        }
    }

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        var opts = options.Value;
        var discovered = sectionStore.Sections;
        var sections = new List<object>();
        decimal divineRate = 0;

        foreach (var section in discovered)
        {
            var apiResponse = await poeNinja.GetSectionAsync(opts.League, section.Type);

            if (divineRate == 0 && apiResponse.Core.Rates.TryGetValue("exalted", out var rate))
                divineRate = rate;

            var nameById = apiResponse.Items.ToDictionary(x => x.Id, x => x.Name);

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
                sections.Add(new { section.Name, section.Group, Items = items });
        }

        cache.Set(CacheKey, new { Sections = sections, DivineOrbRate = divineRate });
    }
}
