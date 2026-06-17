namespace PoeEconomy.Api.Options;

public class EconomyOptions
{
    public const string SectionName = "Economy";
    public string League { get; set; } = "";
    public decimal ThresholdExalts { get; set; } = 50;
}
