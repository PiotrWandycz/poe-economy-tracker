using Microsoft.Extensions.Caching.Memory;
using PoeEconomy.Api;
using PoeEconomy.Api.Options;
using PoeEconomy.Api.PoeNinja;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

builder.Services.Configure<EconomyOptions>(
    builder.Configuration.GetSection(EconomyOptions.SectionName));
builder.Services.AddHttpClient<IPoeNinjaClient, PoeNinjaHttpClient>(c =>
    c.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; PoeEconomy/1.0)"));
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<SectionStore>();
builder.Services.AddSingleton<EconomyCacheService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<EconomyCacheService>());

var app = builder.Build();

app.UseCors();

app.MapGet("/api/economy", (IMemoryCache cache) =>
    cache.Get(EconomyCacheService.CacheKey));

app.MapPost("/api/sections/refresh", async (
    HttpRequest request,
    SectionStore store,
    EconomyCacheService cacheService) =>
{
    using var reader = new StreamReader(request.Body);
    var navHtml = await reader.ReadToEndAsync();
    var sections = PoeNinjaHtmlParser.ParseNavSections(navHtml);
    store.Update(sections);
    await cacheService.RefreshAsync(CancellationToken.None);
    return Results.Ok(new { SectionCount = sections.Count, Sections = sections });
});

app.Run();

public partial class Program { }
