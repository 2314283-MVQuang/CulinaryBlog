using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Features.Recipes.Common;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Enums;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipeBySlug;

public class GetRecipeBySlugQueryHandler(IRecipeRepository recipeRepository)
    : IRequestHandler<GetRecipeBySlugQuery, RecipeDto>
{
    public async Task<RecipeDto> Handle(GetRecipeBySlugQuery request, CancellationToken ct)
    {
        var recipe = await recipeRepository.GetBySlugWithDetailsAsync(request.Slug, ct)
            ?? throw new NotFoundException(nameof(Recipe), request.Slug);

        var isOwnerOrAdmin = request.RequestingUserIsAdmin || recipe.AuthorId == request.RequestingUserId;

        if (recipe.Status != RecipeStatus.Published && !isOwnerOrAdmin)
        {
            // Resource-Based Authorization (mục 2.1): Draft/Archived chỉ tác giả sở hữu hoặc Admin xem được.
            throw new ForbiddenAccessException("Công thức này chưa được công khai.");
        }

        return RecipeMapper.ToDto(recipe);
    }
}
