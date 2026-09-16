using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Features.Recipes.Common;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Ingredients;

public class UpdateRecipeIngredientCommandHandler(
    IRecipeRepository recipeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRecipeIngredientCommand, RecipeIngredientDto>
{
    public async Task<RecipeIngredientDto> Handle(UpdateRecipeIngredientCommand request, CancellationToken ct)
    {
        var recipe = await recipeRepository.GetByIdWithDetailsAsync(request.RecipeId, ct)
            ?? throw new NotFoundException(nameof(Recipe), request.RecipeId);

        RecipeAuthorization.EnsureCanModify(recipe, request.UserId, request.IsAdmin);

        var ingredient = recipe.Ingredients.FirstOrDefault(i => i.Id == request.IngredientId)
            ?? throw new NotFoundException(nameof(RecipeIngredient), request.IngredientId);

        ingredient.Name = request.Name.Trim();
        ingredient.Quantity = request.Quantity;
        ingredient.Unit = string.IsNullOrWhiteSpace(request.Unit) ? null : request.Unit.Trim();
        ingredient.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();

        if (request.OrderIndex is int target && target != ingredient.OrderIndex)
        {
            var desiredOrder = recipe.Ingredients.OrderBy(i => i.OrderIndex).ToList();
            desiredOrder.Remove(ingredient);

            var index = Math.Clamp(target, 0, desiredOrder.Count);
            desiredOrder.Insert(index, ingredient);

            RecipeIngredientOrdering.ApplyOrder(desiredOrder);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return RecipeMapper.ToDto(ingredient);
    }
}
