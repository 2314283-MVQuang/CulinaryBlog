namespace CulinaryBlog.Application.Features.Recipes.Dtos;

/// <summary>Owned Entity Nutrition (mục 5.2) — tất cả nullable, tính trên 1 khẩu phần.</summary>
public record RecipeNutritionDto(
    decimal? Calories,
    decimal? Protein,
    decimal? Carbohydrates,
    decimal? Fat,
    decimal? Fiber,
    decimal? Sodium);
