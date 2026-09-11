using CulinaryBlog.Application.Features.Recipes.Dtos;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipeBySlug;

/// <summary>
/// FR-RCP-002 — GET /api/v1/recipes/{slug}. Recipe Draft/Archived chỉ tác giả sở hữu hoặc Admin
/// được xem (403 nếu không có quyền) — RequestingUserId/RequestingUserIsAdmin lấy từ ICurrentUser
/// ở Endpoint, truyền null/false khi Guest (chưa đăng nhập).
/// </summary>
public record GetRecipeBySlugQuery(string Slug, string? RequestingUserId, bool RequestingUserIsAdmin)
    : IRequest<RecipeDto>;
