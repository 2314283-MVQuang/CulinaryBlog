namespace CulinaryBlog.Application.Features.Recipes.Dtos;

/// <summary>Dùng cho FR-RCP-001 (danh sách) và FR-SRCH-001 (search) — không kèm steps/ingredients đầy đủ.</summary>
public record RecipeListItemDto(
    Guid Id,
    string Title,
    string Slug,
    string Description,
    int PrepTime,
    int CookTime,
    int Servings,
    string Difficulty,
    string Status,
    string CategoryName,
    string AuthorDisplayName,
    string? PrimaryImageUrl,
    DateTimeOffset? PublishedAt);
