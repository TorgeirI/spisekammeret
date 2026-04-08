using Microsoft.EntityFrameworkCore;
using Spisekammeret.Core.Interfaces;
using Spisekammeret.Core.Models;
using Spisekammeret.Infrastructure.Data;

namespace Spisekammeret.Infrastructure.Repositories;

public class RecipeRepository(SpisekammeretDbContext db) : IRecipeRepository
{
    public async Task<IEnumerable<Recipe>> GetAllAsync() =>
        await db.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Tags)
            .Include(r => r.Nutrition)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Recipe?> GetByIdAsync(int id) =>
        await db.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Tags)
            .Include(r => r.Nutrition)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Recipe> CreateAsync(Recipe recipe)
    {
        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();
        return recipe;
    }

    public async Task<Recipe?> UpdateAsync(Recipe recipe)
    {
        var existing = await db.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Tags)
            .Include(r => r.Nutrition)
            .FirstOrDefaultAsync(r => r.Id == recipe.Id);

        if (existing is null) return null;

        db.Entry(existing).CurrentValues.SetValues(recipe);
        await db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var recipe = await db.Recipes.FindAsync(id);
        if (recipe is null) return false;

        db.Recipes.Remove(recipe);
        await db.SaveChangesAsync();
        return true;
    }
}
