using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Enums;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipeBySlug;

public class GetRecipeBySlugQueryHandler(IRecipeRepository recipeRepository)
    : IRequestHandler<GetRecipeBySlugQuery, RecipeDto>
{
    public async Task<RecipeDto> Handle(GetRecipeBySlugQuery request, CancellationToken ct)
    {
        var recipe = await recipeRepository.GetBySlugWithDetailsAsync(request.Slug, ct)
            ?? throw new NotFoundException(nameof(Recipe), request.Slug);

        var isOwnerOrAdmin = request.RequestingUserIsAdmin || recipe.AuthorId == request.RequestingUserId;

        if (recipe.Status != RecipeStatus.Published && !isOwnerOrAdmin)
        {
            // Resource-Based Authorization (mục 2.1): Draft/Archived chỉ tác giả sở hữu hoặc Admin xem được.
            throw new ForbiddenAccessException("Công thức này chưa được công khai.");
        }

        return new RecipeDto(
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
                .ToList(),
            recipe.RowVersion);
    }
}
