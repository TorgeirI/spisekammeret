using AngleSharp;
using AngleSharp.Dom;
using Spisekammeret.Core.Interfaces;

namespace Spisekammeret.Infrastructure.Services;

public class ScraperService : IScraperService
{
    private readonly IBrowsingContext _browser = BrowsingContext.New(Configuration.Default.WithDefaultLoader());

    public async Task<string?> ExtractJsonLdAsync(string url)
    {
        var document = await _browser.OpenAsync(url);

        var scripts = document.QuerySelectorAll("script[type='application/ld+json']");
        foreach (var script in scripts)
        {
            var content = script.TextContent;
            if (content.Contains("Recipe", StringComparison.OrdinalIgnoreCase))
                return content;
        }

        return null;
    }

    public async Task<string> ExtractTextAsync(string url)
    {
        var document = await _browser.OpenAsync(url);

        foreach (var element in document.QuerySelectorAll("script, style, nav, header, footer, aside"))
            element.Remove();

        return document.Body?.TextContent
            .Replace("\t", " ")
            .Replace("\r", "")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => line.Length > 0)
            .Aggregate((a, b) => $"{a}\n{b}") ?? string.Empty;
    }
}
