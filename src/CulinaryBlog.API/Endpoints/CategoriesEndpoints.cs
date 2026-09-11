using CulinaryBlog.API.Extensions;
using CulinaryBlog.Application.Features.Categories.Commands.CreateCategory;
using CulinaryBlog.Application.Features.Categories.Queries.GetCategories;
using MediatR;

namespace CulinaryBlog.API.Endpoints;

/// <summary>
/// Mục 8.2. TODO (nhóm làm tiếp): GET /categories/{slug} (FR-CAT-002), PUT (FR-CAT-004) và
/// DELETE (FR-CAT-005, phải kiểm tra HasRecipesAsync → 409 nếu còn recipe) chưa triển khai —
/// pattern giống hệt CreateCategoryCommand, chỉ khác Command + Handler tương ứng.
/// </summary>
public static class CategoriesEndpoints
{
    public static void MapCategoriesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/categories").WithTags("Categories");

        group.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetCategoriesQuery());
            return result.ToOkResponse();
        });

        group.MapPost("/", async (CreateCategoryCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"/api/v1/categories/{result.Slug}", new { data = result });
        }).RequireAuthorization(AuthorizationPolicies.Admin);
    }
}
