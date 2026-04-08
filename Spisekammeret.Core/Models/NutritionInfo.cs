namespace Spisekammeret.Core.Models;

public class NutritionInfo
{
    public int Id { get; set; }
    public int? Calories { get; set; }
    public double? ProteinGrams { get; set; }
    public double? FatGrams { get; set; }
    public double? CarbohydrateGrams { get; set; }

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
}
