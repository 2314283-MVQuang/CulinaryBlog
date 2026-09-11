namespace CulinaryBlog.Domain.Interfaces;

/// <summary>
/// Gói toàn bộ thay đổi trong 1 request thành 1 transaction duy nhất (mục 4.3: "Mọi mutation
/// đi qua UnitOfWork để đảm bảo tính nhất quán"). Implement bởi CulinaryBlogDbContext ở
/// Infrastructure — SaveChangesAsync() của EF Core vốn đã là 1 transaction.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
