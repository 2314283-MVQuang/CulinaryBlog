namespace CulinaryBlog.Application.Features.Recipes.Dtos;

/// <summary>FR-RCP-002 — chi tiết đầy đủ: Steps, Ingredients, Images, Nutrition, Category, Author.</summary>
public record RecipeDto(
    Guid Id,
    string Title,
    string Slug,
    string Description,
    string Instructions,
    int PrepTime,
    int CookTime,
    int Servings,
    string Difficulty,
    string Status,
    Guid CategoryId,
    string CategoryName,
    string AuthorId,
    string AuthorDisplayName,
    DateTimeOffset? PublishedAt,
    RecipeNutritionDto? Nutrition,
    IReadOnlyList<RecipeStepDto> Steps,
    IReadOnlyList<RecipeIngredientDto> Ingredients,
    IReadOnlyList<RecipeImageDto> Images,
    byte[] RowVersion);

public record RecipeNutritionDto(
    decimal? Calories,
    decimal? Protein,
    decimal? Carbohydrates,
    decimal? Fat,
    decimal? Fiber,
    decimal? Sodium);

public record RecipeStepDto(
    Guid Id,
    int StepNumber,
    string Title,
    string Description,
    int? TimerMinutes,
    string? ImageUrl);

public record RecipeIngredientDto(
    Guid Id,
    string Name,
    decimal? Quantity,
    string? Unit,
    string? Notes,
    int OrderIndex);

public record RecipeImageDto(
    Guid Id,
    string OriginalUrl,
    string? MediumUrl,
    string? ThumbnailUrl,
    string? AltText,
    bool IsPrimary,
    int OrderIndex);
