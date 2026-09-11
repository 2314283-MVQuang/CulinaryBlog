using CulinaryBlog.Domain.Common;

namespace CulinaryBlog.Domain.Interfaces;

/// <summary>
/// Repository chung cho các entity kế thừa BaseEntity. Các repository chuyên biệt
/// (IRecipeRepository, ICategoryRepository...) kế thừa interface này rồi thêm các
/// phương thức truy vấn riêng của mình.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task AddAsync(T entity, CancellationToken ct = default);

    void Update(T entity);

    void Remove(T entity);

    /// <summary>Cho phép Query Handler tự build truy vấn (Include/Where/OrderBy/paging...)
    /// mà không phải thêm phương thức mới vào repository mỗi khi cần lọc kiểu khác.</summary>
    IQueryable<T> Query();
}
