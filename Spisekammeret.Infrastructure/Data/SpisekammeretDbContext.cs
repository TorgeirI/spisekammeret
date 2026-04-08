using Microsoft.EntityFrameworkCore;
using Spisekammeret.Core.Models;

namespace Spisekammeret.Infrastructure.Data;

public class SpisekammeretDbContext(DbContextOptions<SpisekammeretDbContext> options) : DbContext(options)
{
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Recipe>(recipe =>
        {
            recipe.HasKey(r => r.Id);
            recipe.Property(r => r.Name).IsRequired();
            recipe.Property(r => r.Images).HasColumnType("jsonb");
            recipe.Property(r => r.Instructions).HasColumnType("jsonb");
            recipe.Property(r => r.JsonLd).HasColumnType("jsonb");
            recipe.Property(r => r.Difficulty).HasConversion<string>();

            recipe.HasOne(r => r.Nutrition)
                  .WithOne(n => n.Recipe)
                  .HasForeignKey<NutritionInfo>(n => n.RecipeId)
                  .OnDelete(DeleteBehavior.Cascade);

            recipe.HasMany(r => r.Ingredients)
                  .WithOne(i => i.Recipe)
                  .HasForeignKey(i => i.RecipeId)
                  .OnDelete(DeleteBehavior.Cascade);

            recipe.HasMany(r => r.Tags)
                  .WithMany(t => t.Recipes)
                  .UsingEntity(j => j.ToTable("recipe_tags"));
        });

        modelBuilder.Entity<Tag>(tag =>
        {
            tag.HasKey(t => t.Id);
            tag.Property(t => t.Name).IsRequired();
            tag.Property(t => t.Category).IsRequired();
            tag.HasIndex(t => t.Slug).IsUnique();
        });

        modelBuilder.Entity<Ingredient>(ingredient =>
        {
            ingredient.HasKey(i => i.Id);
            ingredient.Property(i => i.Name).IsRequired();
        });
    }
}
