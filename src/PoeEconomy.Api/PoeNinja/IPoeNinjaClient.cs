namespace PoeEconomy.Api.PoeNinja;

public interface IPoeNinjaClient
{
    Task<List<PoeNinjaSection>> GetLeagueSectionsAsync(string league);
    Task<PoeNinjaApiResponse> GetSectionAsync(string league, string sectionType);
}

public record PoeNinjaSection(string Name, string Group, string Type);
public record PoeNinjaApiResponse(PoeNinjaCore Core, List<PoeNinjaLine> Lines);
public record PoeNinjaCore(List<PoeNinjaCoreItem> Items, Dictionary<string, decimal> Rates, string Primary);
public record PoeNinjaCoreItem(string Id, string Name);
public record PoeNinjaLine(string Id, decimal PrimaryValue);
