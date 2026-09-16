using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Enums;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.ChangeStatus;

/// <summary>
/// FR-RCP-005 (publish/unpublish) và FR-RCP-006 (archive) — Actor: Author sở hữu công thức, hoặc Admin.
///
/// Ba endpoint PATCH /recipes/{id}/publish|unpublish|archive cùng dùng MỘT command này, chỉ khác
/// giá trị TargetStatus. Lý do gộp: ba thao tác chỉ khác nhau đúng một dòng gán Status, tách thành
/// ba Handler gần như giống hệt nhau sẽ khó bảo trì hơn (sửa quy tắc quyền phải sửa ba chỗ).
///
/// UserId/IsAdmin lấy từ JWT ở Endpoint, không nhận từ client.
/// </summary>
public record ChangeRecipeStatusCommand(
    Guid RecipeId,
    RecipeStatus TargetStatus,
    string UserId,
    bool IsAdmin) : IRequest<RecipeDto>;
