using PoeEconomy.Api.PoeNinja;

namespace PoeEconomy.Api.Tests;

class FakePoeNinjaClient : IPoeNinjaClient
{
    private readonly Dictionary<string, PoeNinjaApiResponse> _responses = new();
    public int CallCount { get; private set; }

    public void AddSection(string type, PoeNinjaApiResponse response) =>
        _responses[type] = response;

    public Task<PoeNinjaApiResponse> GetSectionAsync(string league, string type)
    {
        CallCount++;
        return Task.FromResult(_responses[type]);
    }
}
