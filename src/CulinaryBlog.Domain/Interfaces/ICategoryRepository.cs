using CulinaryBlog.Domain.Entities;

namespace CulinaryBlog.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug, CancellationToken ct = default);

    Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default);

    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);

    /// <summary>Dùng cho FR-CAT-005: không cho xóa danh mục còn chứa công thức (CATEGORY_DELETE_HAS_RECIPES).</summary>
    Task<bool> HasRecipesAsync(Guid categoryId, CancellationToken ct = default);
}
