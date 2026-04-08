using Microsoft.AspNetCore.Mvc;
using Spisekammeret.Core.Interfaces;

namespace Spisekammeret.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportController(IImportService importService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ImportFromUrl([FromBody] ImportRequest request)
    {
        var recipe = await importService.ImportFromUrlAsync(request.Url);
        return CreatedAtAction("GetById", "Recipes", new { id = recipe.Id }, recipe);
    }
}

public record ImportRequest(string Url);
