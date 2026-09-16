using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Features.Recipes.Common;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Steps;

public class UpdateRecipeStepCommandHandler(
    IRecipeRepository recipeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRecipeStepCommand, RecipeStepDto>
{
    public async Task<RecipeStepDto> Handle(UpdateRecipeStepCommand request, CancellationToken ct)
    {
        var recipe = await recipeRepository.GetByIdWithDetailsAsync(request.RecipeId, ct)
            ?? throw new NotFoundException(nameof(Recipe), request.RecipeId);

        RecipeAuthorization.EnsureCanModify(recipe, request.UserId, request.IsAdmin);

        var step = recipe.Steps.FirstOrDefault(s => s.Id == request.StepId)
            ?? throw new NotFoundException(nameof(RecipeStep), request.StepId);

        step.Title = request.Title.Trim();
        step.Description = request.Description.Trim();
        step.TimerMinutes = request.TimerMinutes;
        step.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();

        await unitOfWork.SaveChangesAsync(ct);

        if (request.StepNumber is int target && target != step.StepNumber)
        {
            MoveTo(recipe, step, target, out var desiredOrder);
            await RecipeStepNumbering.ApplyOrderAsync(desiredOrder, unitOfWork, ct);
        }

        return RecipeMapper.ToDto(step);
    }

    /// <summary>
    /// Dựng danh sách thứ tự MONG MUỐN: rút bước ra khỏi danh sách rồi chèn lại ở vị trí mới.
    /// Vị trí nằm ngoài khoảng hợp lệ thì tự kẹp về đầu/cuối thay vì báo lỗi — client kéo thả
    /// quá tay không nên nhận 422.
    /// </summary>
    private static void MoveTo(Recipe recipe, RecipeStep step, int targetNumber, out List<RecipeStep> desiredOrder)
    {
        desiredOrder = recipe.Steps.OrderBy(s => s.StepNumber).ToList();
        desiredOrder.Remove(step);

        var index = Math.Clamp(targetNumber, 1, desiredOrder.Count + 1) - 1;
        desiredOrder.Insert(index, step);
    }
}
