using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Features.Recipes.Common;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Ingredients;

public class AddRecipeIngredientCommandHandler(
    IRecipeRepository recipeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddRecipeIngredientCommand, RecipeIngredientDto>
{
    public async Task<RecipeIngredientDto> Handle(AddRecipeIngredientCommand request, CancellationToken ct)
    {
        var recipe = await recipeRepository.GetByIdWithDetailsAsync(request.RecipeId, ct)
            ?? throw new NotFoundException(nameof(Recipe), request.RecipeId);

        RecipeAuthorization.EnsureCanModify(recipe, request.UserId, request.IsAdmin);

        var ingredient = new RecipeIngredient
        {
            RecipeId = recipe.Id,
            // OrderIndex đếm từ 0 (khớp DEFAULT 0 của cột trong db/init/02-schema.sql).
            OrderIndex = recipe.Ingredients.Count == 0 ? 0 : recipe.Ingredients.Max(i => i.OrderIndex) + 1,
            Name = request.Name.Trim(),
            Quantity = request.Quantity,
            Unit = string.IsNullOrWhiteSpace(request.Unit) ? null : request.Unit.Trim(),
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
        };

        recipe.Ingredients.Add(ingredient);
        await unitOfWork.SaveChangesAsync(ct);

        return RecipeMapper.ToDto(ingredient);
    }
}
