using System.Security.Claims;
using CulinaryBlog.API.Extensions;
using CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Application.Features.Recipes.Queries.GetRecipeBySlug;
using CulinaryBlog.Application.Features.Recipes.Queries.ListRecipes;
using CulinaryBlog.Domain.Enums;
using MediatR;

namespace CulinaryBlog.API.Endpoints;

/// <summary>
/// Mục 6.3 (Recipes). Mới có FR-RCP-001 (danh sách), FR-RCP-002 (chi tiết), FR-RCP-003 (tạo mới).
/// TODO (nhóm làm tiếp): FR-RCP-004..010 (update/publish/unpublish/archive/delete/images/steps/
/// ingredients) — xem SRS mục 6.3-6.6 khi triển khai.
/// </summary>
public static class RecipesEndpoints
{
    public static void MapRecipesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/recipes").WithTags("Recipes");

        // FR-RCP-001 — công khai, chỉ trả Recipe Status=Published. Tham số truyền thẳng qua query
        // string, ASP.NET Core Minimal API tự bind theo tên + áp giá trị mặc định của C#.
        group.MapGet("/", async (
            ISender sender,
            int page = 1,
            int pageSize = 12,
            string sort = "-createdAt",
            Guid? categoryId = null,
            string? difficulty = null,
            int? maxCookTime = null,
            int? minServings = null) =>
        {
            var query = new ListRecipesQuery(page, pageSize, sort, categoryId, difficulty, maxCookTime, minServings);
            var result = await sender.Send(query);
            return result.ToPagedResponse();
        });

        // FR-RCP-002 — công khai nhưng Draft/Archived chỉ Owner/Admin xem được. KHÔNG gọi
        // .RequireAuthorization() vì Guest cũng được xem Recipe Published; ClaimsPrincipal vẫn có
        // claims nếu client có gửi Bearer token hợp lệ, vì UseAuthentication() chạy cho mọi request
        // bất kể endpoint có RequireAuthorization() hay không.
        group.MapGet("/{slug}", async (string slug, ClaimsPrincipal user, ISender sender) =>
        {
            var query = new GetRecipeBySlugQuery(slug, user.GetUserIdOrNull(), user.IsAdmin());
            var result = await sender.Send(query);
            return result.ToOkResponse();
        });

        // FR-RCP-003 — Author/Admin. AuthorId lấy từ JWT (GetUserId ném 401 nếu thiếu claim), body
        // KHÔNG có field AuthorId để không ai tạo hộ công thức đứng tên người khác.
        group.MapPost("/", async (CreateRecipeRequest request, ClaimsPrincipal user, ISender sender) =>
        {
            var command = new CreateRecipeCommand(
                user.GetUserId(),
                request.Title,
                request.Description,
                request.CategoryId,
                request.PrepTime,
                request.CookTime,
                request.Servings,
                request.Difficulty,
                request.Instructions,
                request.Nutrition);

            var result = await sender.Send(command);
            return Results.Created($"/api/v1/recipes/{result.Slug}", new { data = result });
        }).RequireAuthorization(AuthorizationPolicies.Author);
    }

    private record CreateRecipeRequest(
        string Title,
        string Description,
        Guid CategoryId,
        int PrepTime,
        int CookTime,
        int Servings,
        RecipeDifficulty Difficulty,
        string? Instructions,
        RecipeNutritionDto? Nutrition);
}
