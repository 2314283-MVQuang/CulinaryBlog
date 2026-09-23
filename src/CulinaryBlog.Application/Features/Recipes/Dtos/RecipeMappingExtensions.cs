using CulinaryBlog.Domain.Entities;

namespace CulinaryBlog.Application.Features.Recipes.Dtos;

/// <summary>
/// Map Recipe (Domain) → DTO. Gom về 1 chỗ để ListRecipesQueryHandler, GetRecipeBySlugQueryHandler
/// và CreateRecipeCommandHandler dùng chung, không copy-paste logic map 3 nơi.
///
/// LƯU Ý: gọi các hàm này SAU KHI đã Include() đủ Category/Author/Steps/Ingredients/Images —
/// nếu chưa load navigation property sẽ ném NullReferenceException (Category/Author) hoặc trả
/// danh sách rỗng sai (Steps/Ingredients/Images).
/// </summary>
public static class RecipeMappingExtensions
{
    public static RecipeSummaryDto ToSummaryDto(this Recipe recipe)
    {
        var primaryImage = recipe.Images.FirstOrDefault(i => i.IsPrimary) ?? recipe.Images.FirstOrDefault();

        return new RecipeSummaryDto(
            recipe.Id,
            recipe.Title,
            recipe.Slug,
            recipe.Description,
            primaryImage?.ThumbnailUrl ?? primaryImage?.OriginalUrl,
            recipe.PrepTime,
            recipe.CookTime,
            recipe.Servings,
            recipe.Difficulty,
            recipe.CategoryId,
            recipe.Category.Name,
            recipe.AuthorId,
            recipe.Author.DisplayName,
            recipe.PublishedAt,
            recipe.CreatedAt);
    }

    public static RecipeDetailDto ToDetailDto(this Recipe recipe) => new(
        recipe.Id,
        recipe.Title,
        recipe.Slug,
        recipe.Description,
        recipe.Instructions,
        recipe.PrepTime,
        recipe.CookTime,
        recipe.Servings,
        recipe.Difficulty,
        recipe.Status,
        recipe.CategoryId,
        recipe.Category.Name,
        recipe.AuthorId,
        recipe.Author.DisplayName,
        recipe.PublishedAt,
        recipe.CreatedAt,
        recipe.UpdatedAt,
        new RecipeNutritionDto(
            recipe.Nutrition.Calories,
            recipe.Nutrition.Protein,
            recipe.Nutrition.Carbohydrates,
            recipe.Nutrition.Fat,
            recipe.Nutrition.Fiber,
            recipe.Nutrition.Sodium),
        recipe.Steps
            .OrderBy(s => s.StepNumber)
            .Select(s => new RecipeStepDto(s.Id, s.StepNumber, s.Title, s.Description, s.TimerMinutes, s.ImageUrl))
            .ToList(),
        recipe.Ingredients
            .OrderBy(i => i.OrderIndex)
            .Select(i => new RecipeIngredientDto(i.Id, i.Name, i.Quantity, i.Unit, i.Notes, i.OrderIndex))
            .ToList(),
        recipe.Images
            .OrderBy(i => i.OrderIndex)
            .Select(i => new RecipeImageDto(i.Id, i.OriginalUrl, i.MediumUrl, i.ThumbnailUrl, i.AltText, i.IsPrimary, i.OrderIndex))
            .ToList());
}
