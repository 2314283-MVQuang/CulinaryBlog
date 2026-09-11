using CulinaryBlog.Domain.Common;
using CulinaryBlog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Infrastructure.Persistence.Repositories;

/// <summary>Cài đặt chung cho IRepository&lt;T&gt; — các repository chuyên biệt kế thừa lớp này
/// rồi thêm phương thức riêng (xem CategoryRepository, RecipeRepository).</summary>
public class RepositoryBase<T>(CulinaryBlogDbContext dbContext) : IRepository<T>
    where T : BaseEntity
{
    protected readonly CulinaryBlogDbContext DbContext = dbContext;
    protected readonly DbSet<T> DbSet = dbContext.Set<T>();

    public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        DbSet.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(T entity, CancellationToken ct = default) =>
        await DbSet.AddAsync(entity, ct);

    public void Update(T entity) => DbSet.Update(entity);

    public void Remove(T entity) => DbSet.Remove(entity);

    public IQueryable<T> Query() => DbSet.AsQueryable();
}
