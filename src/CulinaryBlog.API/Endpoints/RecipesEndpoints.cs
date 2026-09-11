using System.Security.Claims;
using CulinaryBlog.API.Extensions;
using CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Application.Features.Recipes.Queries.GetRecipeBySlug;
using CulinaryBlog.Application.Features.Recipes.Queries.GetRecipes;
using CulinaryBlog.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CulinaryBlog.API.Endpoints;

/// <summary>
/// Mục 8.3 — module lõi. TODO (nhóm làm tiếp), theo đúng pattern của CreateRecipe:
///   - PUT /{id} (FR-RCP-004, cần If-Match header + RecipeAuthorizationHandler cho resource-based auth)
///   - PATCH /{id}/publish|unpublish|archive (FR-RCP-005/006)
///   - DELETE /{id} (FR-RCP-007, hard delete + xóa ảnh MinIO qua Hangfire)
///   - GET /recipes/search (FR-SRCH-001, full-text search)
///   - /recipes/{id}/images, /steps, /ingredients (FR-RCP-008/009/010)
/// </summary>
public static class RecipesEndpoints
{
    public static void MapRecipesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/recipes").WithTags("Recipes");

        group.MapGet("/", async ([AsParameters] GetRecipesQuery query, ISender sender) =>
        {
            var result = await sender.Send(query);
            return result.ToPagedResponse();
        });

        group.MapGet("/{slug}", async (string slug, ClaimsPrincipal user, ISender sender) =>
        {
            var userId = user.Identity?.IsAuthenticated == true
                ? user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub")
                : null;

            var result = await sender.Send(new GetRecipeBySlugQuery(slug, userId, user.IsInRole("Admin")));
            return result.ToOkResponse();
        });

        group.MapPost("/", async (CreateRecipeRequest request, ClaimsPrincipal user, ISender sender) =>
        {
            var authorId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub")!;

            var command = new CreateRecipeCommand(
                authorId,
                request.Title,
                request.Description,
                request.Instructions,
                request.CategoryId,
                request.PrepTime,
                request.CookTime,
                request.Servings,
                request.Difficulty,
                request.Nutrition,
                request.Steps,
                request.Ingredients);

            var result = await sender.Send(command);
            return Results.Created($"/api/v1/recipes/{result.Slug}", new { data = result });
        }).RequireAuthorization(AuthorizationPolicies.Author);
    }

    /// <summary>Request body cho POST /recipes — giống CreateRecipeCommand nhưng KHÔNG có AuthorId
    /// (lấy từ JWT, không bao giờ nhận từ client — chống giả mạo tác giả).</summary>
    private record CreateRecipeRequest(
        string Title,
        string Description,
        string Instructions,
        Guid CategoryId,
        int PrepTime,
        int CookTime,
        int Servings,
        RecipeDifficulty Difficulty,
        RecipeNutritionDto? Nutrition,
        IReadOnlyList<CreateRecipeStepInput>? Steps,
        IReadOnlyList<CreateRecipeIngredientInput>? Ingredients);
}
