using CulinaryBlog.Application.Features.Categories.Dtos;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Commands.CreateCategory;

/// <summary>FR-CAT-003 — POST /api/v1/categories. Chỉ Admin (kiểm tra qua policy "AdminPolicy" ở endpoint).</summary>
public record CreateCategoryCommand(
    string Name,
    string? Description,
    string? ImageUrl,
    int OrderIndex) : IRequest<CategoryDto>;
