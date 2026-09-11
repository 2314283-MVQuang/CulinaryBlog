using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Common.Helpers;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Enums;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;

public class CreateRecipeCommandHandler(
    IRecipeRepository recipeRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateRecipeCommand, RecipeDto>
{
    public async Task<RecipeDto> Handle(CreateRecipeCommand request, CancellationToken ct)
    {
        var category = await categoryRepository.GetByIdAsync(request.CategoryId, ct)
            ?? throw new NotFoundException(nameof(Category), request.CategoryId);

        var recipe = new Recipe
        {
            Title = request.Title,
            Slug = await GenerateUniqueSlugAsync(request.Title, ct),
            Description = request.Description,
            Instructions = request.Instructions,
            CategoryId = request.CategoryId,
            AuthorId = request.AuthorId,
            PrepTime = request.PrepTime,
            CookTime = request.CookTime,
            Servings = request.Servings,
            Difficulty = request.Difficulty,
            Status = RecipeStatus.Draft, // FR-RCP-003: trạng thái ban đầu LUÔN là Draft.
            Nutrition = new RecipeNutrition
            {
                Calories = request.Nutrition?.Calories,
                Protein = request.Nutrition?.Protein,
                Carbohydrates = request.Nutrition?.Carbohydrates,
                Fat = request.Nutrition?.Fat,
                Fiber = request.Nutrition?.Fiber,
                Sodium = request.Nutrition?.Sodium,
            },
        };

        foreach (var step in request.Steps ?? [])
        {
            recipe.Steps.Add(new RecipeStep
            {
                StepNumber = step.StepNumber,
                Title = step.Title,
                Description = step.Description,
                TimerMinutes = step.TimerMinutes,
                ImageUrl = step.ImageUrl,
            });
        }

        foreach (var ingredient in request.Ingredients ?? [])
        {
            recipe.Ingredients.Add(new RecipeIngredient
            {
                Name = ingredient.Name,
                Quantity = ingredient.Quantity,
                Unit = ingredient.Unit,
                Notes = ingredient.Notes,
                OrderIndex = ingredient.OrderIndex,
            });
        }

        await recipeRepository.AddAsync(recipe, ct);
        await unitOfWork.SaveChangesAsync(ct);

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
            category.Name,
            recipe.AuthorId,
            AuthorDisplayName: string.Empty, // TODO: nạp Author.DisplayName nếu cần hiển thị ngay sau khi tạo
            recipe.PublishedAt,
            request.Nutrition,
            recipe.Steps.OrderBy(s => s.StepNumber)
                .Select(s => new RecipeStepDto(s.Id, s.StepNumber, s.Title, s.Description, s.TimerMinutes, s.ImageUrl))
                .ToList(),
            recipe.Ingredients.OrderBy(i => i.OrderIndex)
                .Select(i => new RecipeIngredientDto(i.Id, i.Name, i.Quantity, i.Unit, i.Notes, i.OrderIndex))
                .ToList(),
            [],
            recipe.RowVersion);
    }

    private async Task<string> GenerateUniqueSlugAsync(string title, CancellationToken ct)
    {
        var baseSlug = SlugHelper.GenerateSlug(title);
        var slug = baseSlug;
        var suffix = 2;

        while (await recipeRepository.SlugExistsAsync(slug, ct))
        {
            slug = SlugHelper.AppendSuffix(baseSlug, suffix++);
        }

        return slug;
    }
}
