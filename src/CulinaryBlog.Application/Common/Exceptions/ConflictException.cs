namespace CulinaryBlog.Application.Common.Exceptions;

/// <summary>
/// Map sang 409 Conflict ở GlobalExceptionMiddleware. Dùng cho các business rule kiểu:
/// email/slug/name đã tồn tại, hoặc CATEGORY_DELETE_HAS_RECIPES (mục 10.2 — TODO: nhóm bổ sung
/// đủ mã lỗi theo bảng 10.2 khi có nhu cầu, hiện dùng message chung).
/// </summary>
public sealed class ConflictException(string message) : Exception(message);
