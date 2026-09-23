namespace CulinaryBlog.Application.Features.Recipes.Dtos;

/// <summary>Một ảnh của công thức (mục 5.5, FR-RCP-008 — chưa triển khai upload, chỉ dùng để đọc).</summary>
public record RecipeImageDto(
    Guid Id,
    string OriginalUrl,
    string? MediumUrl,
    string? ThumbnailUrl,
    string? AltText,
    bool IsPrimary,
    int OrderIndex);
