using CulinaryBlog.Domain.Entities;

namespace CulinaryBlog.Domain.Interfaces;

public interface IRecipeRepository : IRepository<Recipe>
{
    /// <summary>Eager load Steps, Ingredients, Images, Category, Author (FR-RCP-002).</summary>
    Task<Recipe?> GetBySlugWithDetailsAsync(string slug, CancellationToken ct = default);

    Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default);
}
