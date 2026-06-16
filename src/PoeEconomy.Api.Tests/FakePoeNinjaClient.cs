using PoeEconomy.Api.PoeNinja;

namespace PoeEconomy.Api.Tests;

class FakePoeNinjaClient : IPoeNinjaClient
{
    private readonly Dictionary<string, PoeNinjaApiResponse> _responses = new();

    public void AddSection(string type, PoeNinjaApiResponse response) =>
        _responses[type] = response;

    public Task<PoeNinjaApiResponse> GetSectionAsync(string league, string type) =>
        Task.FromResult(_responses[type]);
}
