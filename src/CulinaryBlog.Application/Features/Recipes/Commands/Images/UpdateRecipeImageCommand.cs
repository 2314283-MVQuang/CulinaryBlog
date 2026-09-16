using CulinaryBlog.Application.Features.Recipes.Dtos;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Images;

/// <summary>
/// FR-RCP-008 — PUT /api/v1/recipes/{id}/images/{imageId}. Chỉ sửa THÔNG TIN của ảnh
/// (mô tả, thứ tự, có phải ảnh đại diện không); muốn đổi file ảnh thì xoá rồi upload ảnh mới.
///
/// IsPrimary để kiểu bool? chứ không phải bool: nếu để bool, client chỉ muốn sửa mô tả mà quên
/// gửi field này thì JSON deserialize ra false và ẢNH ĐẠI DIỆN BỊ GỠ MẤT một cách âm thầm.
/// Với bool?, không gửi (null) = giữ nguyên, giống quy ước của OrderIndex.
/// </summary>
public record UpdateRecipeImageCommand(
    Guid RecipeId,
    Guid ImageId,
    string? AltText,
    bool? IsPrimary,
    int? OrderIndex,
    string UserId,
    bool IsAdmin) : IRequest<RecipeImageDto>;
