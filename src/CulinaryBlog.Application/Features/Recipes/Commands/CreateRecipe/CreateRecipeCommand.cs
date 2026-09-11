using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Enums;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;

/// <summary>
/// FR-RCP-003 — POST /api/v1/recipes (Actor: Author/Admin). Trạng thái ban đầu LUÔN là Draft.
/// AuthorId lấy từ ICurrentUser ở Endpoint (JWT), không nhận từ client.
/// </summary>
public record CreateRecipeCommand(
    string AuthorId,
    string Title,
    string Description,
    string Instructions,
    Guid CategoryId,
    int PrepTime,
    int CookTime,
    int Servings,
    RecipeDifficulty Difficulty,
    RecipeNutritionDto? Nutrition,
    IReadOnlyList<CreateRecipeStepInput>? Steps,
    IReadOnlyList<CreateRecipeIngredientInput>? Ingredients) : IRequest<RecipeDto>;

public record CreateRecipeStepInput(int StepNumber, string Title, string Description, int? TimerMinutes, string? ImageUrl);

public record CreateRecipeIngredientInput(string Name, decimal? Quantity, string? Unit, string? Notes, int OrderIndex);
