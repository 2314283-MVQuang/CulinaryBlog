using CulinaryBlog.Domain.Enums;

namespace CulinaryBlog.Application.Features.Recipes.Dtos;

/// <summary>
/// Chi tiết đầy đủ 1 công thức (FR-RCP-002) — kèm Steps/Ingredients/Images/Nutrition. Cũng dùng
/// làm response cho FR-RCP-003 (tạo mới) để không cần thêm 1 DTO tương tự.
/// </summary>
public record RecipeDetailDto(
    Guid Id,
    string Title,
    string Slug,
    string Description,
    string? Instructions,
    int PrepTime,
    int CookTime,
    int Servings,
    RecipeDifficulty Difficulty,
    RecipeStatus Status,
    Guid CategoryId,
    string CategoryName,
    string AuthorId,
    string AuthorDisplayName,
    DateTimeOffset? PublishedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    RecipeNutritionDto Nutrition,
    IReadOnlyList<RecipeStepDto> Steps,
    IReadOnlyList<RecipeIngredientDto> Ingredients,
    IReadOnlyList<RecipeImageDto> Images);
