using CulinaryBlog.Application.Features.Recipes.Dtos;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Ingredients;

/// <summary>
/// FR-RCP-009 — POST /api/v1/recipes/{id}/ingredients. Nguyên liệu mới luôn nối vào cuối danh sách.
/// </summary>
public record AddRecipeIngredientCommand(
    Guid RecipeId,
    string Name,
    decimal? Quantity,
    string? Unit,
    string? Notes,
    string UserId,
    bool IsAdmin) : IRequest<RecipeIngredientDto>;
