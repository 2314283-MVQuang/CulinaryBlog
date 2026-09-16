using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Infrastructure.Persistence.Repositories;

public class RecipeRepository(CulinaryBlogDbContext dbContext)
    : RepositoryBase<Recipe>(dbContext), IRecipeRepository
{
    public Task<Recipe?> GetBySlugWithDetailsAsync(string slug, CancellationToken ct = default) =>
        WithDetails().FirstOrDefaultAsync(r => r.Slug == slug, ct);

    public Task<Recipe?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default) =>
        WithDetails().FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default) =>
        DbSet.AnyAsync(x => x.Slug == slug, ct);

    /// <summary>
    /// Gom phần Include dùng chung cho cả hai cách tra cứu. Lưu ý: các entity con được EF Core
    /// theo dõi (tracked) nên Handler sửa trực tiếp Steps/Ingredients/Images rồi gọi
    /// SaveChangesAsync là đủ, không cần gọi Update() cho từng dòng con.
    /// </summary>
    private IQueryable<Recipe> WithDetails() =>
        DbSet
            .Include(r => r.Category)
            .Include(r => r.Author)
            .Include(r => r.Steps)
            .Include(r => r.Ingredients)
            .Include(r => r.Images);
}
