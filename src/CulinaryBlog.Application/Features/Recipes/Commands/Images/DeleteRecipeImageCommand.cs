using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Images;

/// <summary>
/// FR-RCP-008 — DELETE /api/v1/recipes/{id}/images/{imageId}. Xoá cả dòng trong DB lẫn file
/// trên kho lưu trữ. Nếu ảnh bị xoá đang là ảnh đại diện thì ảnh còn lại đứng đầu danh sách
/// được đôn lên thay thế.
/// </summary>
public record DeleteRecipeImageCommand(
    Guid RecipeId,
    Guid ImageId,
    string UserId,
    bool IsAdmin) : IRequest;
