namespace CulinaryBlog.Application.Features.Recipes.Dtos;

/// <summary>Một nguyên liệu (mục 5.4, FR-RCP-009). Quantity/Unit đều có thể null (nguyên liệu "vừa đủ").</summary>
public record RecipeIngredientDto(
    Guid Id,
    string Name,
    decimal? Quantity,
    string? Unit,
    string? Notes,
    int OrderIndex);
