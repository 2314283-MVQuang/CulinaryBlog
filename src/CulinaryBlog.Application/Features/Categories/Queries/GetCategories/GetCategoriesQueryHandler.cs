using CulinaryBlog.Application.Features.Categories.Dtos;
using CulinaryBlog.Domain.Enums;
using CulinaryBlog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct) =>
        categoryRepository.Query()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.ImageUrl,
                c.Recipes.Count(r => r.Status == RecipeStatus.Published)))
            .ToListAsync(ct);
}
