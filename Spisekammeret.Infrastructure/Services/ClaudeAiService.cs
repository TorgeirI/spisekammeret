using System.Text.Json;
using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using Spisekammeret.Core.Models;

namespace Spisekammeret.Infrastructure.Services;

public class ClaudeAiService(AnthropicClient client)
{
    public async Task<Recipe> ParseRecipeAsync(string content)
    {
        var prompt = """
            Analyser følgende oppskriftsinnhold og returner KUN gyldig JSON (ingen forklaring, ingen markdown).
            Navn og beskrivelse skal være på norsk.

            JSON-skjema:
            {
              "name": "string",
              "description": "string",
              "servings": integer,
              "prepTimeMinutes": integer,
              "cookTimeMinutes": integer,
              "cuisine": "string",
              "difficulty": "Enkel" eller "Middels" eller "Krevende",
              "ingredients": ["mengde enhet ingrediens", ...],
              "instructions": ["steg 1", "steg 2", ...]
            }

            Innhold:
            """ + content;

        var messages = new List<Message>
        {
            new()
            {
                Role = RoleType.User,
                Content = [new TextContent { Text = prompt }]
            }
        };

        var parameters = new MessageParameters
        {
            Model = AnthropicModels.Claude46Sonnet,
            MaxTokens = 2048,
            Messages = messages
        };

        var response = await client.Messages.GetClaudeMessageAsync(parameters);
        var text = response.Content.OfType<TextContent>().First().Text;

        var json = JsonSerializer.Deserialize<JsonElement>(text);
        return MapToRecipe(json);
    }

    private static Recipe MapToRecipe(JsonElement input)
    {
        var recipe = new Recipe
        {
            Name        = input.GetProperty("name").GetString()!,
            Description = input.TryGetProperty("description", out var desc) ? desc.GetString() : null,
            Servings    = input.TryGetProperty("servings", out var srv)  && srv.ValueKind == JsonValueKind.Number ? srv.GetInt32() : null,
            Cuisine     = input.TryGetProperty("cuisine", out var cui)   ? cui.GetString() : null,
        };

        if (input.TryGetProperty("prepTimeMinutes", out var prep) && prep.ValueKind == JsonValueKind.Number && prep.GetInt32() > 0)
            recipe.PrepTime = TimeSpan.FromMinutes(prep.GetInt32());

        if (input.TryGetProperty("cookTimeMinutes", out var cook) && cook.ValueKind == JsonValueKind.Number && cook.GetInt32() > 0)
            recipe.CookTime = TimeSpan.FromMinutes(cook.GetInt32());

        if (input.TryGetProperty("difficulty", out var diff) &&
            Enum.TryParse<Difficulty>(diff.GetString(), out var difficulty))
            recipe.Difficulty = difficulty;

        if (input.TryGetProperty("ingredients", out var ingredients))
            recipe.Ingredients = ingredients.EnumerateArray()
                .Select(i => new Ingredient { Name = i.GetString()! })
                .ToList();

        if (input.TryGetProperty("instructions", out var instructions))
            recipe.Instructions = instructions.EnumerateArray()
                .Select(s => s.GetString()!)
                .ToList();

        return recipe;
    }
}
