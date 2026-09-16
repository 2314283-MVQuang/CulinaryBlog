namespace CulinaryBlog.Domain.Interfaces;

/// <summary>
/// Gói toàn bộ thay đổi trong 1 request thành 1 transaction duy nhất (mục 4.3: "Mọi mutation
/// đi qua UnitOfWork để đảm bảo tính nhất quán"). Implement bởi CulinaryBlogDbContext ở
/// Infrastructure — SaveChangesAsync() của EF Core vốn đã là 1 transaction.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    /// <summary>
    /// Chạy nhiều lần SaveChangesAsync trong CÙNG một transaction — hoặc tất cả cùng thành công,
    /// hoặc DB quay về y như trước khi gọi.
    ///
    /// Vì sao cần: một vài thao tác buộc phải lưu làm nhiều lần để không vi phạm ràng buộc UNIQUE
    /// của PostgreSQL (đánh số lại các bước nấu, đổi ảnh đại diện — xem RecipeStepNumbering).
    /// Nếu để mỗi lần lưu là một transaction riêng, sự cố giữa chừng sẽ để lại dữ liệu dở dang
    /// (các bước mang số tạm, hoặc công thức không còn ảnh đại diện nào).
    ///
    /// LƯU Ý: không gọi lồng nhau — bên trong action đừng gọi lại ExecuteInTransactionAsync.
    /// </summary>
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default);
}
