namespace Spisekammeret.Core.Models;

public class Tag
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public string Slug { get; set; } = string.Empty;

    public ICollection<Recipe> Recipes { get; set; } = [];
}
