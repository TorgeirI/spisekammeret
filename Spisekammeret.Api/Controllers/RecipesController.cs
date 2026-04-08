using Microsoft.AspNetCore.Mvc;
using Spisekammeret.Core.Interfaces;
using Spisekammeret.Core.Models;

namespace Spisekammeret.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController(IRecipeRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await repository.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var recipe = await repository.GetByIdAsync(id);
        return recipe is null ? NotFound() : Ok(recipe);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Recipe recipe)
    {
        var created = await repository.CreateAsync(recipe);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Recipe recipe)
    {
        if (id != recipe.Id) return BadRequest();
        var updated = await repository.UpdateAsync(recipe);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await repository.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
