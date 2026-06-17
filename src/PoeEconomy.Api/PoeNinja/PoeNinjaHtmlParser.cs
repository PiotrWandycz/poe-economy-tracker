using HtmlAgilityPack;

namespace PoeEconomy.Api.PoeNinja;

public static class PoeNinjaHtmlParser
{
    public static List<PoeNinjaSection> ParseNavSections(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var sections = new List<PoeNinjaSection>();
        var groupHeaders = doc.DocumentNode.SelectNodes("//h3[@data-variant='subdued']");
        if (groupHeaders == null) return sections;

        foreach (var h3 in groupHeaders)
        {
            var group = HtmlEntity.DeEntitize(h3.InnerText.Trim());
            var ul = h3.SelectSingleNode("following-sibling::ul[1]");
            if (ul == null) continue;

            var links = ul.SelectNodes(".//a");
            if (links == null) continue;
            foreach (var a in links)
            {
                var href = a.GetAttributeValue("href", "");
                var name = a.GetAttributeValue("title", "");
                if (string.IsNullOrEmpty(href) || string.IsNullOrEmpty(name)) continue;

                var slug = href.Split('/').Last();
                var type = SlugToType(slug);
                sections.Add(new PoeNinjaSection(name, group, type));
            }
        }

        return sections;
    }

    private static string SlugToType(string slug) =>
        string.Concat(slug.Split('-').Select(w => w.Length > 0 ? char.ToUpper(w[0]) + w[1..] : w));
}
