using CulinaryBlog.Domain.Common;
using CulinaryBlog.Domain.Enums;

namespace CulinaryBlog.Domain.Entities;

/// <summary>
/// Công thức nấu ăn — aggregate root của hệ thống (bảng "Recipes" — mục 7.2).
/// Chứa các child entity: Steps, Ingredients, Images và Owned Entity Nutrition.
/// Mọi thay đổi phải đi qua IUnitOfWork để đảm bảo tính transaction (mục 4.3).
/// </summary>
public class Recipe : BaseEntity
{
    public string Title { get; set; } = null!;

    /// <summary>Sinh từ Title, KHÔNG đổi sau khi Publish (SEO — mục 5.7).</summary>
    public string Slug { get; set; } = null!;

    /// <summary>Mô tả ngắn, ≤2000 ký tự (validate ở FluentValidation, không validate ở đây).</summary>
    public string Description { get; set; } = null!;

    /// <summary>Hướng dẫn tổng quan dạng markdown — trường "legacy", chi tiết từng bước dùng Steps.</summary>
    public string Instructions { get; set; } = null!;

    /// <summary>Phút.</summary>
    public int PrepTime { get; set; }

    /// <summary>Phút, 0 cho "No cook".</summary>
    public int CookTime { get; set; }

    public int Servings { get; set; }

    public RecipeDifficulty Difficulty { get; set; } = RecipeDifficulty.Easy;

    public RecipeStatus Status { get; set; } = RecipeStatus.Draft;

    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public string AuthorId { get; set; } = null!;

    public ApplicationUser Author { get; set; } = null!;

    /// <summary>
    /// CỘT NÀY KHÔNG ĐƯỢC MAP VÀO C# (xem RecipeConfiguration.cs): cột "SearchVector" (tsvector)
    /// trong PostgreSQL được trigger "trg_Recipes_search_vector" tự cập nhật từ Title +
    /// Description mỗi khi INSERT/UPDATE (xem db/init/02-schema.sql) — EF Core không cần và
    /// không nên đụng vào.
    /// </summary>
    public DateTimeOffset? PublishedAt { get; set; }

    public RecipeNutrition Nutrition { get; set; } = new();

    public ICollection<RecipeStep> Steps { get; set; } = [];

    public ICollection<RecipeIngredient> Ingredients { get; set; } = [];

    public ICollection<RecipeImage> Images { get; set; } = [];
}
