using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Features.Recipes.Common;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Steps;

public class AddRecipeStepCommandHandler(
    IRecipeRepository recipeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddRecipeStepCommand, RecipeStepDto>
{
    public async Task<RecipeStepDto> Handle(AddRecipeStepCommand request, CancellationToken ct)
    {
        var recipe = await recipeRepository.GetByIdWithDetailsAsync(request.RecipeId, ct)
            ?? throw new NotFoundException(nameof(Recipe), request.RecipeId);

        RecipeAuthorization.EnsureCanModify(recipe, request.UserId, request.IsAdmin);

        var step = new RecipeStep
        {
            RecipeId = recipe.Id,
            // Luôn nối vào cuối: tránh phải đánh số lại cả danh sách chỉ để thêm 1 bước.
            StepNumber = recipe.Steps.Count == 0 ? 1 : recipe.Steps.Max(s => s.StepNumber) + 1,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            TimerMinutes = request.TimerMinutes,
            ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim(),
        };

        // Thêm vào collection của recipe đang được EF theo dõi — EF tự sinh câu INSERT khi SaveChanges.
        recipe.Steps.Add(step);
        await unitOfWork.SaveChangesAsync(ct);

        return RecipeMapper.ToDto(step);
    }
}
