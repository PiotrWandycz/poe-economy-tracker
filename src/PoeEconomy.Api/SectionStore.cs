using PoeEconomy.Api.PoeNinja;

namespace PoeEconomy.Api;

public class SectionStore
{
    private List<PoeNinjaSection> _sections = [];

    public IReadOnlyList<PoeNinjaSection> Sections => _sections;

    public void Update(List<PoeNinjaSection> sections) => _sections = sections;
}
