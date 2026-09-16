using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Ingredients;

/// <summary>FR-RCP-009 — DELETE /api/v1/recipes/{id}/ingredients/{ingredientId}.</summary>
public record DeleteRecipeIngredientCommand(
    Guid RecipeId,
    Guid IngredientId,
    string UserId,
    bool IsAdmin) : IRequest;
