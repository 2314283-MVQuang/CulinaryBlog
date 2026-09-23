using CulinaryBlog.Application.Features.Recipes.Dtos;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipeBySlug;

/// <summary>
/// FR-RCP-002 — GET /api/v1/recipes/{slug} (công khai, không bắt buộc đăng nhập). CurrentUserId/
/// IsCurrentUserAdmin do Endpoint đọc từ JWT claims rồi truyền vào (giống pattern GetMeQuery) —
/// Application Layer không phụ thuộc ClaimsPrincipal/ASP.NET Core.
/// </summary>
public record GetRecipeBySlugQuery(
    string Slug,
    string? CurrentUserId,
    bool IsCurrentUserAdmin) : IRequest<RecipeDetailDto>;
