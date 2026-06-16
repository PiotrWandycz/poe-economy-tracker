namespace PoeEconomy.Api.Options;

public class EconomyOptions
{
    public const string SectionName = "Economy";
    public string League { get; set; } = "";
    public decimal ThresholdExalts { get; set; } = 50;
    public List<SectionConfig> Sections { get; set; } = [];
}

public class SectionConfig
{
    public string Type { get; set; } = "";
    public string Name { get; set; } = "";
    public string Group { get; set; } = "General";
}
