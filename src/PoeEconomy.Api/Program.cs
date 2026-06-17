using Microsoft.Extensions.Caching.Memory;
using PoeEconomy.Api;
using PoeEconomy.Api.Options;
using PoeEconomy.Api.PoeNinja;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EconomyOptions>(
    builder.Configuration.GetSection(EconomyOptions.SectionName));
builder.Services.AddHttpClient<IPoeNinjaClient, PoeNinjaHttpClient>(c =>
    c.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; PoeEconomy/1.0)"));
builder.Services.AddMemoryCache();
builder.Services.AddHostedService<EconomyCacheService>();

var app = builder.Build();

app.MapGet("/api/economy", (IMemoryCache cache) =>
    cache.Get(EconomyCacheService.CacheKey));

app.Run();

public partial class Program { }
