using Spisekammeret.Core.Models;

namespace Spisekammeret.Core.Interfaces;

public interface IRecipeRepository
{
    Task<IEnumerable<Recipe>> GetAllAsync();
    Task<Recipe?> GetByIdAsync(int id);
    Task<Recipe> CreateAsync(Recipe recipe);
    Task<Recipe?> UpdateAsync(Recipe recipe);
    Task<bool> DeleteAsync(int id);
}
