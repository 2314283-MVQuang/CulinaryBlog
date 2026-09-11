namespace CulinaryBlog.Application.Common.Interfaces;

/// <summary>
/// Abstraction cho lưu trữ file (FR-FILE-001/002, mục 4.5): cho phép swap implementation
/// (local filesystem ↔ MinIO ↔ AWS S3) mà không cần đổi Application Layer.
///
/// TODO (nhóm làm tiếp): bản hiện tại (LocalFileStorageService ở Infrastructure) chỉ lưu vào
/// đĩa cục bộ để scaffold chạy được ngay không cần MinIO. Khi triển khai MinIO thật, viết thêm
/// MinioFileStorageService implement interface NÀY (không sửa Application Layer) rồi đổi
/// đăng ký DI ở Infrastructure/DependencyInjection.cs.
/// </summary>
public interface IFileStorageService
{
    /// <summary>Trả về public URL. Tên file luôn là GUID mới (chống path traversal — FR-FILE-001).</summary>
    Task<string> UploadAsync(Stream content, string originalFileName, string contentType, string folder, CancellationToken ct = default);

    /// <summary>Idempotent: nếu file không tồn tại thì không throw.</summary>
    Task DeleteAsync(string fileUrl, CancellationToken ct = default);
}
