using CulinaryBlog.Application.Features.Recipes.Dtos;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Steps;

/// <summary>
/// FR-RCP-010 — POST /api/v1/recipes/{id}/steps. Bước mới luôn được thêm vào CUỐI danh sách
/// (StepNumber = số lớn nhất hiện có + 1). Muốn chèn vào giữa thì thêm vào cuối rồi gọi
/// PUT .../steps/{stepId} với stepNumber mong muốn để dời lên.
/// </summary>
public record AddRecipeStepCommand(
    Guid RecipeId,
    string Title,
    string Description,
    int? TimerMinutes,
    string? ImageUrl,
    string UserId,
    bool IsAdmin) : IRequest<RecipeStepDto>;
