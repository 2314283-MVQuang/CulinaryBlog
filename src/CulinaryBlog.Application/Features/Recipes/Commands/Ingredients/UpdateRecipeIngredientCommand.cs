using CulinaryBlog.Application.Features.Recipes.Dtos;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Ingredients;

/// <summary>
/// FR-RCP-009 — PUT /api/v1/recipes/{id}/ingredients/{ingredientId}.
/// Cùng quy ước với bước nấu: Name bắt buộc, Quantity/Unit/Notes gửi null = xoá giá trị cũ,
/// riêng OrderIndex null = giữ nguyên vị trí.
/// </summary>
public record UpdateRecipeIngredientCommand(
    Guid RecipeId,
    Guid IngredientId,
    string Name,
    decimal? Quantity,
    string? Unit,
    string? Notes,
    int? OrderIndex,
    string UserId,
    bool IsAdmin) : IRequest<RecipeIngredientDto>;
