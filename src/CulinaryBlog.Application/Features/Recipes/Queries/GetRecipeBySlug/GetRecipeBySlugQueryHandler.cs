using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Enums;
using CulinaryBlog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipeBySlug;

public class GetRecipeBySlugQueryHandler(IRepository<Recipe> recipeRepository)
    : IRequestHandler<GetRecipeBySlugQuery, RecipeDetailDto>
{
    public async Task<RecipeDetailDto> Handle(GetRecipeBySlugQuery request, CancellationToken ct)
    {
        var recipe = await recipeRepository.Query()
            .Include(r => r.Category)
            .Include(r => r.Author)
            .Include(r => r.Steps)
            .Include(r => r.Ingredients)
            .Include(r => r.Images)
            .FirstOrDefaultAsync(r => r.Slug == request.Slug, ct);

        // FR-RCP-002: Draft/Archived chỉ Owner/Admin xem được. Cùng 1 lỗi RECIPE_NOT_FOUND cho cả
        // "không tồn tại" lẫn "có tồn tại nhưng không có quyền xem" — không lộ thông tin tồn tại
        // của Draft người khác.
        if (recipe is null || !CanView(recipe, request))
        {
            throw new NotFoundException(nameof(Recipe), request.Slug);
        }

        return recipe.ToDetailDto();
    }

    private static bool CanView(Recipe recipe, GetRecipeBySlugQuery request)
    {
        if (recipe.Status == RecipeStatus.Published)
        {
            return true;
        }

        var isOwner = request.CurrentUserId is not null && request.CurrentUserId == recipe.AuthorId;
        return isOwner || request.IsCurrentUserAdmin;
    }
}
