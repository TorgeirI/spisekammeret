namespace Spisekammeret.Core.Interfaces;

public interface IScraperService
{
    Task<string?> ExtractJsonLdAsync(string url);
    Task<string> ExtractTextAsync(string url);
}
