using CulinaryBlog.Domain.Entities;

namespace CulinaryBlog.Domain.Interfaces;

public interface IRecipeRepository : IRepository<Recipe>
{
    /// <summary>Eager load Steps, Ingredients, Images, Category, Author (FR-RCP-002).</summary>
    Task<Recipe?> GetBySlugWithDetailsAsync(string slug, CancellationToken ct = default);

    /// <summary>
    /// Giống GetBySlugWithDetailsAsync nhưng tra theo Id — dùng cho các lệnh sửa đổi
    /// (đổi trạng thái FR-RCP-005/006, CRUD ảnh/nguyên liệu/bước FR-RCP-008/009/010),
    /// vì client gửi lên Id chứ không phải slug.
    /// </summary>
    Task<Recipe?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);

    Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default);
}
