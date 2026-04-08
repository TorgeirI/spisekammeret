using Anthropic.SDK;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spisekammeret.Core.Interfaces;
using Spisekammeret.Infrastructure.Data;
using Spisekammeret.Infrastructure.Repositories;
using Spisekammeret.Infrastructure.Services;

namespace Spisekammeret.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SpisekammeretDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IScraperService, ScraperService>();
        services.AddScoped<IImportService, ImportService>();

        services.AddScoped<ClaudeAiService>();
        services.AddScoped(_ => new AnthropicClient(configuration["Anthropic:ApiKey"]!));

        return services;
    }
}
