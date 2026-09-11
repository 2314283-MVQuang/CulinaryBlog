using CulinaryBlog.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CulinaryBlog.Infrastructure.Services;

/// <summary>
/// TODO (nhóm làm tiếp): implementation TẠM THỜI của IFileStorageService — lưu file vào đĩa cục
/// bộ (thư mục "wwwroot/uploads") thay vì MinIO, để scaffold chạy được ngay không cần thêm
/// service MinIO trong Docker Compose. Khi triển khai FR-FILE-001/002 thật:
///   1. Thêm service "minio" vào docker-compose.yml (xem mục 6.4 tài liệu đặc tả).
///   2. Viết MinioFileStorageService implement IFileStorageService (dùng AWSSDK.S3), validate
///      MIME type qua magic bytes + giới hạn 5MB (CONS-007) — bản này CHƯA validate.
///   3. Đổi đăng ký DI ở Infrastructure/DependencyInjection.cs sang MinioFileStorageService.
/// </summary>
public class LocalFileStorageService(IConfiguration configuration) : IFileStorageService
{
    private readonly string _rootPath = configuration["FileStorage:LocalRootPath"] ?? "wwwroot/uploads";
    private readonly string _publicBaseUrl = configuration["FileStorage:PublicBaseUrl"] ?? "/uploads";

    public async Task<string> UploadAsync(Stream content, string originalFileName, string contentType, string folder, CancellationToken ct = default)
    {
        var extension = Path.GetExtension(originalFileName);
        // Tên file luôn là GUID mới — chống path traversal (FR-FILE-001).
        var fileName = $"{Guid.NewGuid()}{extension}";

        var folderPath = Path.Combine(_rootPath, folder);
        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);
        await using (var fileStream = File.Create(filePath))
        {
            await content.CopyToAsync(fileStream, ct);
        }

        return $"{_publicBaseUrl}/{folder}/{fileName}";
    }

    public Task DeleteAsync(string fileUrl, CancellationToken ct = default)
    {
        var relativePath = fileUrl.Replace(_publicBaseUrl, string.Empty).TrimStart('/');
        var filePath = Path.Combine(_rootPath, relativePath);

        // Idempotent: file không tồn tại thì không throw (FR-FILE-002).
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
