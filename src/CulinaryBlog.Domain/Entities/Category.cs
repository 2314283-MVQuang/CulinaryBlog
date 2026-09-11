using CulinaryBlog.Domain.Common;

namespace CulinaryBlog.Domain.Entities;

/// <summary>Danh mục công thức (bảng "Categories" — mục 7.6). CRUD chỉ dành cho Admin.</summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = null!;

    /// <summary>Sinh tự động từ Name (SlugHelper), KHÔNG đổi khi đổi tên để tránh broken link (FR-CAT-004).</summary>
    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int OrderIndex { get; set; }

    public ICollection<Recipe> Recipes { get; set; } = [];
}
