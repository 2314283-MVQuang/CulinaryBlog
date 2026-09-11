namespace CulinaryBlog.Domain.Common;

/// <summary>
/// Lớp cha cho mọi entity trong hệ thống (mục 7.1 tài liệu đặc tả).
///
/// Ba cột cuối được QUẢN LÝ BỞI DATABASE, không phải bởi code C#:
///   - CreatedAt / UpdatedAt: AuditInterceptor (xem Infrastructure/Persistence/Interceptors)
///     set khi SaveChanges, nhưng PostgreSQL cũng có trigger "touch_row()" (xem
///     db/init/02-schema.sql) tự cập nhật UpdatedAt mỗi lần UPDATE — hai lớp bảo vệ này
///     không xung đột nhau.
///   - RowVersion: KHÔNG bao giờ set tay. Trigger "touch_row()" sinh giá trị ngẫu nhiên mới
///     mỗi lần UPDATE, EF Core đọc lại giá trị đó sau khi lưu (xem cấu hình IsRowVersion()
///     trong các file *Configuration.cs) để dùng cho optimistic concurrency (If-Match header).
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Soft delete flag. LƯU Ý: Recipe áp dụng HARD DELETE theo FR-RCP-007 (xóa thật, cascade
    /// xóa Steps/Ingredients/Images), nên với Recipe cột này luôn là false trong thực tế —
    /// vẫn giữ vì Recipe kế thừa BaseEntity như các entity khác.
    /// </summary>
    public bool IsDeleted { get; set; }

    public byte[] RowVersion { get; set; } = [];
}
