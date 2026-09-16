using CulinaryBlog.Application.Features.Recipes.Dtos;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Steps;

/// <summary>
/// FR-RCP-010 — PUT /api/v1/recipes/{id}/steps/{stepId}.
///
/// Dùng PUT (thay thế toàn bộ nội dung bước) chứ không PATCH: Title và Description bắt buộc gửi,
/// TimerMinutes/ImageUrl gửi null nghĩa là XOÁ giá trị cũ. Như vậy không cần quy ước rắc rối
/// "null là giữ nguyên hay là xoá".
///
/// Riêng StepNumber là ngoại lệ: null = giữ nguyên vị trí hiện tại, có giá trị = dời bước tới
/// vị trí đó và tự đánh số lại cả danh sách.
/// </summary>
public record UpdateRecipeStepCommand(
    Guid RecipeId,
    Guid StepId,
    string Title,
    string Description,
    int? TimerMinutes,
    string? ImageUrl,
    int? StepNumber,
    string UserId,
    bool IsAdmin) : IRequest<RecipeStepDto>;
