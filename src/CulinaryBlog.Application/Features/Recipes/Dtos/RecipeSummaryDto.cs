using CulinaryBlog.Domain.Enums;

namespace CulinaryBlog.Application.Features.Recipes.Dtos;

/// <summary>
/// Dùng cho danh sách/lưới công thức (FR-RCP-001) — nhẹ hơn RecipeDetailDto, KHÔNG kèm
/// Steps/Ingredients/Images đầy đủ, chỉ ảnh đại diện (ThumbnailUrl).
/// </summary>
public record RecipeSummaryDto(
    Guid Id,
    string Title,
    string Slug,
    string Description,
    string? ThumbnailUrl,
    int PrepTime,
    int CookTime,
    int Servings,
    RecipeDifficulty Difficulty,
    Guid CategoryId,
    string CategoryName,
    string AuthorId,
    string AuthorDisplayName,
    DateTimeOffset? PublishedAt,
    DateTimeOffset CreatedAt);
