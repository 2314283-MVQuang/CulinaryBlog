using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Queries.ListRecipes;

/// <summary>
/// FR-RCP-001 — GET /api/v1/recipes (Actor: Guest, công khai). Chỉ trả Recipe Status=Published.
/// Filter kết hợp AND (FR-SRCH-002), sort DUY NHẤT qua tham số "sort" (FR-SRCH-003, mặc định
/// "-createdAt"), phân trang offset-based (FR-SRCH-004, mặc định page=1/pageSize=12, tối đa 50).
/// </summary>
public record ListRecipesQuery(
    int Page = 1,
    int PageSize = 12,
    string Sort = "-createdAt",
    Guid? CategoryId = null,
    string? Difficulty = null,
    int? MaxCookTime = null,
    int? MinServings = null) : IRequest<PagedResult<RecipeSummaryDto>>;
