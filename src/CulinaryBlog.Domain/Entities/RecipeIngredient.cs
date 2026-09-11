using CulinaryBlog.Domain.Common;

namespace CulinaryBlog.Domain.Entities;

/// <summary>Một nguyên liệu trong công thức (bảng "RecipeIngredients" — mục 7.4, FR-RCP-009).</summary>
public class RecipeIngredient : BaseEntity
{
    public Guid RecipeId { get; set; }

    public Recipe Recipe { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal? Quantity { get; set; }

    /// <summary>gram/ml/muỗng/củ...</summary>
    public string? Unit { get; set; }

    public string? Notes { get; set; }

    public int OrderIndex { get; set; }
}
