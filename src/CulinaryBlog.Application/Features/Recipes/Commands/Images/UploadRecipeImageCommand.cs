using CulinaryBlog.Application.Features.Recipes.Dtos;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Images;

/// <summary>
/// FR-RCP-008 — POST /api/v1/recipes/{id}/images (multipart/form-data).
///
/// Application Layer cố tình nhận <see cref="Stream"/> chứ không nhận IFormFile: IFormFile là kiểu
/// của ASP.NET Core, nếu để lọt vào đây thì tầng Application phụ thuộc Presentation, phá Dependency
/// Rule (mục 3). Endpoint chịu trách nhiệm mở stream từ IFormFile rồi truyền xuống.
///
/// Ảnh đầu tiên của công thức tự động thành ảnh đại diện (IsPrimary = true).
/// </summary>
public record UploadRecipeImageCommand(
    Guid RecipeId,
    Stream Content,
    string FileName,
    string ContentType,
    long SizeInBytes,
    string? AltText,
    string UserId,
    bool IsAdmin) : IRequest<RecipeImageDto>;
