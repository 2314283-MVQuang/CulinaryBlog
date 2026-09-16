using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Entities;

namespace CulinaryBlog.Application.Features.Recipes.Common;

/// <summary>
/// Chuyển entity sang DTO. Trước đây đoạn map RecipeDto dài ~30 dòng nằm lặp trong từng Handler;
/// gom về đây để sửa DTO một chỗ là mọi endpoint cùng đổi theo.
///
/// LƯU Ý khi dùng ToDto(Recipe): recipe phải được nạp kèm Category, Author, Steps, Ingredients,
/// Images (dùng IRecipeRepository.GetByIdWithDetailsAsync / GetBySlugWithDetailsAsync), nếu
/// không sẽ ném NullReferenceException ở recipe.Category.Name.
/// </summary>
public static class RecipeMapper
{
    public static RecipeDto ToDto(Recipe recipe) => new(
        recipe.Id,
        recipe.Title,
        recipe.Slug,
        recipe.Description,
        recipe.Instructions,
        recipe.PrepTime,
        recipe.CookTime,
        recipe.Servings,
        recipe.Difficulty.ToString(),
        recipe.Status.ToString(),
        recipe.CategoryId,
        recipe.Category.Name,
        recipe.AuthorId,
        recipe.Author.DisplayName,
        recipe.PublishedAt,
        new RecipeNutritionDto(
            recipe.Nutrition.Calories,
            recipe.Nutrition.Protein,
            recipe.Nutrition.Carbohydrates,
            recipe.Nutrition.Fat,
            recipe.Nutrition.Fiber,
            recipe.Nutrition.Sodium),
        recipe.Steps.OrderBy(s => s.StepNumber).Select(s => ToDto(s)).ToList(),
        recipe.Ingredients.OrderBy(i => i.OrderIndex).Select(i => ToDto(i)).ToList(),
        recipe.Images.OrderBy(i => i.OrderIndex).Select(i => ToDto(i)).ToList(),
        recipe.RowVersion);

    public static RecipeStepDto ToDto(RecipeStep step) => new(
        step.Id,
        step.StepNumber,
        step.Title,
        step.Description,
        step.TimerMinutes,
        step.ImageUrl);

    public static RecipeIngredientDto ToDto(RecipeIngredient ingredient) => new(
        ingredient.Id,
        ingredient.Name,
        ingredient.Quantity,
        ingredient.Unit,
        ingredient.Notes,
        ingredient.OrderIndex);

    public static RecipeImageDto ToDto(RecipeImage image) => new(
        image.Id,
        image.OriginalUrl,
        image.MediumUrl,
        image.ThumbnailUrl,
        image.AltText,
        image.IsPrimary,
        image.OrderIndex);
}
