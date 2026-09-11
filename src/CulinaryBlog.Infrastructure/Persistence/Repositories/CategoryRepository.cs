using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Infrastructure.Persistence.Repositories;

public class CategoryRepository(CulinaryBlogDbContext dbContext)
    : RepositoryBase<Category>(dbContext), ICategoryRepository
{
    public Task<Category?> GetBySlugAsync(string slug, CancellationToken ct = default) =>
        DbSet.FirstOrDefaultAsync(x => x.Slug == slug, ct);

    public Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default) =>
        DbSet.AnyAsync(x => x.Slug == slug, ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default) =>
        DbSet.AnyAsync(x => x.Name == name, ct);

    public Task<bool> HasRecipesAsync(Guid categoryId, CancellationToken ct = default) =>
        DbContext.Recipes.AnyAsync(r => r.CategoryId == categoryId, ct);
}
