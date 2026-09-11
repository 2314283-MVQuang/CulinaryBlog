using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Enums;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipes;

/// <summary>
/// FR-RCP-001 — GET /api/v1/recipes?page&amp;pageSize&amp;categoryId&amp;difficulty&amp;maxCookTime&amp;sort.
///
/// TODO (nhóm làm tiếp): bản hiện tại LUÔN chỉ trả Status=Published (đúng cho Guest). Theo
/// FR-RCP-001, Author phải thấy thêm Draft/Archived của chính mình và Admin thấy tất cả — cần
/// truyền ICurrentUser vào Handler và mở rộng điều kiện lọc. Cũng chưa có Output Cache
/// (TTL 15 phút, mục 6.3) — thêm khi tích hợp Redis.
/// </summary>
public record GetRecipesQuery(
    int Page = 1,
    int PageSize = 12,
    Guid? CategoryId = null,
    RecipeDifficulty? Difficulty = null,
    int? MaxCookTime = null,
    string Sort = "-createdAt") : IRequest<PagedResult<RecipeListItemDto>>;
