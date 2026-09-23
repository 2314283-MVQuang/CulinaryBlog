using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Enums;
using CulinaryBlog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Recipes.Queries.ListRecipes;

public class ListRecipesQueryHandler(IRepository<Recipe> recipeRepository)
    : IRequestHandler<ListRecipesQuery, PagedResult<RecipeSummaryDto>>
{
    public async Task<PagedResult<RecipeSummaryDto>> Handle(ListRecipesQuery request, CancellationToken ct)
    {
        var query = recipeRepository.Query()
            .Include(r => r.Category)
            .Include(r => r.Author)
            .Include(r => r.Images)
            .Where(r => r.Status == RecipeStatus.Published);

        if (request.CategoryId is { } categoryId)
        {
            query = query.Where(r => r.CategoryId == categoryId);
        }

        // Validator đã chặn giá trị không hợp lệ nên TryParse ở đây luôn thành công khi có giá trị.
        if (request.Difficulty is not null && Enum.TryParse<RecipeDifficulty>(request.Difficulty, true, out var difficulty))
        {
            query = query.Where(r => r.Difficulty == difficulty);
        }

        if (request.MaxCookTime is { } maxCookTime)
        {
            query = query.Where(r => r.CookTime <= maxCookTime);
        }

        if (request.MinServings is { } minServings)
        {
            query = query.Where(r => r.Servings >= minServings);
        }

        query = ApplySort(query, request.Sort);

        var totalCount = await query.CountAsync(ct);

        var recipes = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var items = recipes.Select(r => r.ToSummaryDto()).ToList();

        return PagedResult<RecipeSummaryDto>.Create(items, request.Page, request.PageSize, totalCount);
    }

    /// <summary>
    /// FR-SRCH-003: DUY NHẤT tham số "sort", dấu "-" ở đầu = giảm dần. ListRecipesQueryValidator
    /// đã whitelist field nên nhánh mặc định (_, ...) chỉ còn khớp "createdAt" — không có field lạ
    /// nào lọt tới đây.
    /// </summary>
    private static IQueryable<Recipe> ApplySort(IQueryable<Recipe> query, string sort)
    {
        var descending = sort.StartsWith('-');
        var field = sort.TrimStart('-');

        return (field, descending) switch
        {
            ("title", false) => query.OrderBy(r => r.Title),
            ("title", true) => query.OrderByDescending(r => r.Title),
            ("prepTime", false) => query.OrderBy(r => r.PrepTime),
            ("prepTime", true) => query.OrderByDescending(r => r.PrepTime),
            ("cookTime", false) => query.OrderBy(r => r.CookTime),
            ("cookTime", true) => query.OrderByDescending(r => r.CookTime),
            ("servings", false) => query.OrderBy(r => r.Servings),
            ("servings", true) => query.OrderByDescending(r => r.Servings),
            ("publishedAt", false) => query.OrderBy(r => r.PublishedAt),
            ("publishedAt", true) => query.OrderByDescending(r => r.PublishedAt),
            (_, false) => query.OrderBy(r => r.CreatedAt),
            (_, true) => query.OrderByDescending(r => r.CreatedAt),
        };
    }
}
