namespace CulinaryBlog.Application.Features.Categories.Dtos;

/// <summary>FR-CAT-001: kèm số lượng recipe Published trong danh mục.</summary>
public record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? ImageUrl,
    int RecipeCount);
