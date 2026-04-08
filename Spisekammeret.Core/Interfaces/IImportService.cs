using Spisekammeret.Core.Models;

namespace Spisekammeret.Core.Interfaces;

public interface IImportService
{
    Task<Recipe> ImportFromUrlAsync(string url);
}
