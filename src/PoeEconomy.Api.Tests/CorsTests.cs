using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PoeEconomy.Api;
using PoeEconomy.Api.PoeNinja;

namespace PoeEconomy.Api.Tests;

public class CorsTests
{
    // The Vite dev server runs on http://localhost:5173 by default.
    // Requests from it to the .NET backend must include CORS headers.
    [Fact]
    public async Task Economy_endpoint_returns_cors_header_for_allowed_origin()
    {
        var sections = new SectionStore(); // empty — no poe.ninja calls made

        await using var app = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b
                .ConfigureTestServices(services =>
                    services.AddSingleton(sections))
                .ConfigureAppConfiguration((_, cfg) =>
                {
                    cfg.Sources.Clear();
                    cfg.AddInMemoryCollection(new Dictionary<string, string?> {
                        ["Economy:League"] = "Test League",
                        ["Economy:ThresholdExalts"] = "0",
                        ["Cors:AllowedOrigins:0"] = "http://localhost:5173"
                    });
                }));

        var client = app.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/economy");
        request.Headers.Add("Origin", "http://localhost:5173");

        var response = await client.SendAsync(request);

        var corsHeader = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
        Assert.Equal("http://localhost:5173", corsHeader);
    }
}
