namespace Spisekammeret.Core.Models;

public class Recipe
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<string> Images { get; set; } = [];

    public TimeSpan? PrepTime { get; set; }
    public TimeSpan? CookTime { get; set; }
    public TimeSpan? TotalTime { get; set; }

    public int? Servings { get; set; }
    public string? Cuisine { get; set; }
    public Difficulty? Difficulty { get; set; }

    public List<string> Instructions { get; set; } = [];

    public NutritionInfo? Nutrition { get; set; }

    public string? SourceUrl { get; set; }
    public string? JsonLd { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Ingredient> Ingredients { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];
}
