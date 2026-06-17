using PoeEconomy.Api.PoeNinja;

namespace PoeEconomy.Api.Tests;

class FakePoeNinjaClient : IPoeNinjaClient
{
    private readonly Dictionary<string, PoeNinjaApiResponse> _responses = new();
    private List<PoeNinjaSection> _sections = [];
    public int CallCount { get; private set; }

    public void AddSection(string type, PoeNinjaApiResponse response) =>
        _responses[type] = response;

    public void SetSections(List<PoeNinjaSection> sections) =>
        _sections = sections;

    public Task<List<PoeNinjaSection>> GetLeagueSectionsAsync(string league) =>
        Task.FromResult(_sections);

    public Task<PoeNinjaApiResponse> GetSectionAsync(string league, string type)
    {
        CallCount++;
        return Task.FromResult(_responses[type]);
    }
}
