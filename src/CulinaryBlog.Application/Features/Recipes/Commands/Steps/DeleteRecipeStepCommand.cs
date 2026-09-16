using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Steps;

/// <summary>
/// FR-RCP-010 — DELETE /api/v1/recipes/{id}/steps/{stepId}. Xoá xong các bước còn lại được
/// đánh số lại liên tục 1..n (không để lỗ hổng kiểu "Bước 1, Bước 3").
/// </summary>
public record DeleteRecipeStepCommand(
    Guid RecipeId,
    Guid StepId,
    string UserId,
    bool IsAdmin) : IRequest;
