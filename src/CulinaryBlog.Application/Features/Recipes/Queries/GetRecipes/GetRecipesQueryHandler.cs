using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Enums;
using CulinaryBlog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipes;

public class GetRecipesQueryHandler(IRecipeRepository recipeRepository)
    : IRequestHandler<GetRecipesQuery, PagedResult<RecipeListItemDto>>
{
    public async Task<PagedResult<RecipeListItemDto>> Handle(GetRecipesQuery request, CancellationToken ct)
    {
        var query = recipeRepository.Query()
            .Where(r => r.Status == RecipeStatus.Published); // TODO: mở rộng theo role, xem ghi chú ở GetRecipesQuery

        if (request.CategoryId.HasValue)
        {
            query = query.Where(r => r.CategoryId == request.CategoryId.Value);
        }

        if (request.Difficulty.HasValue)
        {
            query = query.Where(r => r.Difficulty == request.Difficulty.Value);
        }

        if (request.MaxCookTime.HasValue)
        {
            query = query.Where(r => r.CookTime <= request.MaxCookTime.Value);
        }

        query = ApplySort(query, request.Sort);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new RecipeListItemDto(
                r.Id,
                r.Title,
                r.Slug,
                r.Description,
                r.PrepTime,
                r.CookTime,
                r.Servings,
                r.Difficulty.ToString(),
                r.Status.ToString(),
                r.Category.Name,
                r.Author.DisplayName,
                r.Images.Where(i => i.IsPrimary).Select(i => i.ThumbnailUrl ?? i.OriginalUrl).FirstOrDefault(),
                r.PublishedAt))
            .ToListAsync(ct);

        return PagedResult<RecipeListItemDto>.Create(items, request.Page, request.PageSize, totalCount);
    }

    /// <summary>Sort mặc định "-createdAt" (mới nhất trước). Tiền tố "-" = giảm dần (mục 4.4).</summary>
    private static IQueryable<Recipe> ApplySort(IQueryable<Recipe> query, string sort)
    {
        var descending = sort.StartsWith('-');
        var field = descending ? sort[1..] : sort;

        return field.ToLowerInvariant() switch
        {
            "title" => descending ? query.OrderByDescending(r => r.Title) : query.OrderBy(r => r.Title),
            "cooktime" => descending ? query.OrderByDescending(r => r.CookTime) : query.OrderBy(r => r.CookTime),
            _ => descending ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
        };
    }
}
