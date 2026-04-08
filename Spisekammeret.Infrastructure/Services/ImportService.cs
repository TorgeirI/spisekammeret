using Spisekammeret.Core.Interfaces;
using Spisekammeret.Core.Models;

namespace Spisekammeret.Infrastructure.Services;

public class ImportService(IScraperService scraper, ClaudeAiService claude, IRecipeRepository repository) : IImportService
{
    public async Task<Recipe> ImportFromUrlAsync(string url)
    {
        var jsonLd = await scraper.ExtractJsonLdAsync(url);

        Recipe recipe;
        if (jsonLd is not null)
            recipe = await claude.ParseRecipeAsync(jsonLd);
        else
        {
            var text = await scraper.ExtractTextAsync(url);
            recipe = await claude.ParseRecipeAsync(text);
        }

        recipe.SourceUrl = url;
        return await repository.CreateAsync(recipe);
    }
}
