using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Features.Recipes.Common;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Ingredients;

public class DeleteRecipeIngredientCommandHandler(
    IRecipeRepository recipeRepository,
    IRepository<RecipeIngredient> ingredientRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteRecipeIngredientCommand>
{
    public async Task Handle(DeleteRecipeIngredientCommand request, CancellationToken ct)
    {
        var recipe = await recipeRepository.GetByIdWithDetailsAsync(request.RecipeId, ct)
            ?? throw new NotFoundException(nameof(Recipe), request.RecipeId);

        RecipeAuthorization.EnsureCanModify(recipe, request.UserId, request.IsAdmin);

        var ingredient = recipe.Ingredients.FirstOrDefault(i => i.Id == request.IngredientId)
            ?? throw new NotFoundException(nameof(RecipeIngredient), request.IngredientId);

        recipe.Ingredients.Remove(ingredient);
        ingredientRepository.Remove(ingredient);

        // Không vướng ràng buộc UNIQUE nên xoá và đánh số lại gọn trong cùng một SaveChanges.
        RecipeIngredientOrdering.ApplyOrder(recipe.Ingredients.OrderBy(i => i.OrderIndex).ToList());

        await unitOfWork.SaveChangesAsync(ct);
    }
}
