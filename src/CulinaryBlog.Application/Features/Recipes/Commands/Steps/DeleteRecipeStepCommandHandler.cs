using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Features.Recipes.Common;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Steps;

public class DeleteRecipeStepCommandHandler(
    IRecipeRepository recipeRepository,
    IRepository<RecipeStep> stepRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteRecipeStepCommand>
{
    public async Task Handle(DeleteRecipeStepCommand request, CancellationToken ct)
    {
        var recipe = await recipeRepository.GetByIdWithDetailsAsync(request.RecipeId, ct)
            ?? throw new NotFoundException(nameof(Recipe), request.RecipeId);

        RecipeAuthorization.EnsureCanModify(recipe, request.UserId, request.IsAdmin);

        var step = recipe.Steps.FirstOrDefault(s => s.Id == request.StepId)
            ?? throw new NotFoundException(nameof(RecipeStep), request.StepId);

        // Bỏ khỏi collection trong bộ nhớ + đánh dấu xoá ở DbSet. Làm cả hai để vừa đúng dữ liệu
        // đang giữ trên RAM (dùng ngay ở bước đánh số lại bên dưới), vừa rõ ý đồ với EF Core.
        recipe.Steps.Remove(step);
        stepRepository.Remove(step);
        await unitOfWork.SaveChangesAsync(ct);

        await RecipeStepNumbering.ApplyOrderAsync(
            recipe.Steps.OrderBy(s => s.StepNumber).ToList(),
            unitOfWork,
            ct);
    }
}
