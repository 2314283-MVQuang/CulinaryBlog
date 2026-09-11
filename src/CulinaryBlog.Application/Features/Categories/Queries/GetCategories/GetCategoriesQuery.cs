using CulinaryBlog.Application.Features.Categories.Dtos;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Queries.GetCategories;

/// <summary>
/// FR-CAT-001 — GET /api/v1/categories (public). Sắp xếp theo Name tăng dần.
/// TODO (nhóm làm tiếp): thêm CachingBehavior (Redis, TTL 60 phút, cache key "categories:all")
/// khi tích hợp Redis (mục 5.1) — hiện query thẳng DB mỗi lần gọi.
/// </summary>
public record GetCategoriesQuery : IRequest<List<CategoryDto>>;
