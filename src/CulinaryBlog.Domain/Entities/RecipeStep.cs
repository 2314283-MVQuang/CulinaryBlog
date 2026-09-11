using CulinaryBlog.Domain.Common;

namespace CulinaryBlog.Domain.Entities;

/// <summary>Một bước thực hiện trong công thức (bảng "RecipeSteps" — mục 7.3, FR-RCP-010).</summary>
public class RecipeStep : BaseEntity
{
    public Guid RecipeId { get; set; }

    public Recipe Recipe { get; set; } = null!;

    /// <summary>Thứ tự bước (1,2,3...), duy nhất trong cùng 1 Recipe. Tự renumber khi xóa 1 bước.</summary>
    public int StepNumber { get; set; }

    public string Title { get; set; } = null!;

    /// <summary>≤2000 ký tự.</summary>
    public string Description { get; set; } = null!;

    public int? TimerMinutes { get; set; }

    public string? ImageUrl { get; set; }
}
