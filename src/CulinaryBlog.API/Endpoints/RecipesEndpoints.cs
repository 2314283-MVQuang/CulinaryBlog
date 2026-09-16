using System.Security.Claims;
using CulinaryBlog.API.Extensions;
using CulinaryBlog.Application.Features.Recipes.Commands.ChangeStatus;
using CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;
using CulinaryBlog.Application.Features.Recipes.Commands.Images;
using CulinaryBlog.Application.Features.Recipes.Commands.Ingredients;
using CulinaryBlog.Application.Features.Recipes.Commands.Steps;
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
///   - DELETE /{id} (FR-RCP-007, hard delete + xoá ảnh trên kho lưu trữ)
///   - GET /recipes/search (FR-SRCH-001, full-text search)
///
/// Đã triển khai: đổi trạng thái (FR-RCP-005/006) và CRUD ảnh/nguyên liệu/bước (FR-RCP-008/009/010).
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
            var result = await sender.Send(new GetRecipeBySlugQuery(slug, user.GetUserIdOrNull(), user.IsAdmin()));
            return result.ToOkResponse();
        });

        group.MapPost("/", async (CreateRecipeRequest request, ClaimsPrincipal user, ISender sender) =>
        {
            var command = new CreateRecipeCommand(
                user.GetUserId(),
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

        MapStatusEndpoints(group);
        MapStepEndpoints(group);
        MapIngredientEndpoints(group);
        MapImageEndpoints(group);
    }

    // ---------------------------------------------------------------------
    // FR-RCP-005 / FR-RCP-006 — đổi trạng thái
    // ---------------------------------------------------------------------
    private static void MapStatusEndpoints(RouteGroupBuilder group)
    {
        // Ba đường dẫn riêng cho ba hành động (thay vì một endpoint nhận status trong body) để
        // URL tự mô tả việc nó làm và phân quyền/ghi log về sau tách bạch được từng hành động.
        group.MapPatch("/{id:guid}/publish", (Guid id, ClaimsPrincipal user, ISender sender) =>
            ChangeStatusAsync(id, RecipeStatus.Published, user, sender))
            .RequireAuthorization(AuthorizationPolicies.Author);

        group.MapPatch("/{id:guid}/unpublish", (Guid id, ClaimsPrincipal user, ISender sender) =>
            ChangeStatusAsync(id, RecipeStatus.Draft, user, sender))
            .RequireAuthorization(AuthorizationPolicies.Author);

        group.MapPatch("/{id:guid}/archive", (Guid id, ClaimsPrincipal user, ISender sender) =>
            ChangeStatusAsync(id, RecipeStatus.Archived, user, sender))
            .RequireAuthorization(AuthorizationPolicies.Author);
    }

    private static async Task<IResult> ChangeStatusAsync(
        Guid id,
        RecipeStatus targetStatus,
        ClaimsPrincipal user,
        ISender sender)
    {
        var command = new ChangeRecipeStatusCommand(id, targetStatus, user.GetUserId(), user.IsAdmin());
        var result = await sender.Send(command);
        return result.ToOkResponse();
    }

    // ---------------------------------------------------------------------
    // FR-RCP-010 — các bước thực hiện
    // ---------------------------------------------------------------------
    private static void MapStepEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/steps", async (Guid id, AddStepRequest request, ClaimsPrincipal user, ISender sender) =>
        {
            var command = new AddRecipeStepCommand(
                id,
                request.Title,
                request.Description,
                request.TimerMinutes,
                request.ImageUrl,
                user.GetUserId(),
                user.IsAdmin());

            var result = await sender.Send(command);
            return Results.Created($"/api/v1/recipes/{id}/steps/{result.Id}", new { data = result });
        }).RequireAuthorization(AuthorizationPolicies.Author);

        group.MapPut("/{id:guid}/steps/{stepId:guid}", async (Guid id, Guid stepId, UpdateStepRequest request, ClaimsPrincipal user, ISender sender) =>
        {
            var command = new UpdateRecipeStepCommand(
                id,
                stepId,
                request.Title,
                request.Description,
                request.TimerMinutes,
                request.ImageUrl,
                request.StepNumber,
                user.GetUserId(),
                user.IsAdmin());

            var result = await sender.Send(command);
            return result.ToOkResponse();
        }).RequireAuthorization(AuthorizationPolicies.Author);

        group.MapDelete("/{id:guid}/steps/{stepId:guid}", async (Guid id, Guid stepId, ClaimsPrincipal user, ISender sender) =>
        {
            await sender.Send(new DeleteRecipeStepCommand(id, stepId, user.GetUserId(), user.IsAdmin()));
            return Results.NoContent();
        }).RequireAuthorization(AuthorizationPolicies.Author);
    }

    // ---------------------------------------------------------------------
    // FR-RCP-009 — nguyên liệu
    // ---------------------------------------------------------------------
    private static void MapIngredientEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/ingredients", async (Guid id, AddIngredientRequest request, ClaimsPrincipal user, ISender sender) =>
        {
            var command = new AddRecipeIngredientCommand(
                id,
                request.Name,
                request.Quantity,
                request.Unit,
                request.Notes,
                user.GetUserId(),
                user.IsAdmin());

            var result = await sender.Send(command);
            return Results.Created($"/api/v1/recipes/{id}/ingredients/{result.Id}", new { data = result });
        }).RequireAuthorization(AuthorizationPolicies.Author);

        group.MapPut("/{id:guid}/ingredients/{ingredientId:guid}", async (Guid id, Guid ingredientId, UpdateIngredientRequest request, ClaimsPrincipal user, ISender sender) =>
        {
            var command = new UpdateRecipeIngredientCommand(
                id,
                ingredientId,
                request.Name,
                request.Quantity,
                request.Unit,
                request.Notes,
                request.OrderIndex,
                user.GetUserId(),
                user.IsAdmin());

            var result = await sender.Send(command);
            return result.ToOkResponse();
        }).RequireAuthorization(AuthorizationPolicies.Author);

        group.MapDelete("/{id:guid}/ingredients/{ingredientId:guid}", async (Guid id, Guid ingredientId, ClaimsPrincipal user, ISender sender) =>
        {
            await sender.Send(new DeleteRecipeIngredientCommand(id, ingredientId, user.GetUserId(), user.IsAdmin()));
            return Results.NoContent();
        }).RequireAuthorization(AuthorizationPolicies.Author);
    }

    // ---------------------------------------------------------------------
    // FR-RCP-008 — ảnh
    // ---------------------------------------------------------------------
    private static void MapImageEndpoints(RouteGroupBuilder group)
    {
        // DisableAntiforgery(): từ .NET 8, Minimal API nhận IFormFile mặc định đòi antiforgery token
        // (cơ chế chống CSRF dựa trên cookie). API này xác thực bằng Bearer token chứ không dùng
        // cookie nên không có nguy cơ CSRF, và client (Next.js) không có token đó để gửi.
        group.MapPost("/{id:guid}/images", async (Guid id, IFormFile file, HttpContext http, ClaimsPrincipal user, ISender sender) =>
        {
            var altText = http.Request.Form["altText"].ToString();

            await using var content = file.OpenReadStream();

            var command = new UploadRecipeImageCommand(
                id,
                content,
                file.FileName,
                file.ContentType,
                file.Length,
                string.IsNullOrWhiteSpace(altText) ? null : altText,
                user.GetUserId(),
                user.IsAdmin());

            var result = await sender.Send(command);
            return Results.Created($"/api/v1/recipes/{id}/images/{result.Id}", new { data = result });
        })
        .RequireAuthorization(AuthorizationPolicies.Author)
        .DisableAntiforgery();

        group.MapPut("/{id:guid}/images/{imageId:guid}", async (Guid id, Guid imageId, UpdateImageRequest request, ClaimsPrincipal user, ISender sender) =>
        {
            var command = new UpdateRecipeImageCommand(
                id,
                imageId,
                request.AltText,
                request.IsPrimary,
                request.OrderIndex,
                user.GetUserId(),
                user.IsAdmin());

            var result = await sender.Send(command);
            return result.ToOkResponse();
        }).RequireAuthorization(AuthorizationPolicies.Author);

        group.MapDelete("/{id:guid}/images/{imageId:guid}", async (Guid id, Guid imageId, ClaimsPrincipal user, ISender sender) =>
        {
            await sender.Send(new DeleteRecipeImageCommand(id, imageId, user.GetUserId(), user.IsAdmin()));
            return Results.NoContent();
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

    private record AddStepRequest(string Title, string Description, int? TimerMinutes, string? ImageUrl);

    private record UpdateStepRequest(string Title, string Description, int? TimerMinutes, string? ImageUrl, int? StepNumber);

    private record AddIngredientRequest(string Name, decimal? Quantity, string? Unit, string? Notes);

    private record UpdateIngredientRequest(string Name, decimal? Quantity, string? Unit, string? Notes, int? OrderIndex);

    /// <summary>isPrimary không gửi (null) = giữ nguyên ảnh đại diện hiện tại.</summary>
    private record UpdateImageRequest(string? AltText, bool? IsPrimary, int? OrderIndex);
}
