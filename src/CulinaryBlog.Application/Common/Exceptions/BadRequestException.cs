namespace CulinaryBlog.Application.Common.Exceptions;

/// <summary>
/// Map sang 400 Bad Request ở GlobalExceptionMiddleware. Dùng cho lỗi request sai về mặt NGHIỆP VỤ
/// nhưng không thuộc lỗi validate input thông thường (422) — ví dụ AUTH_GOOGLE_TOKEN_INVALID khi
/// Google idToken sai chữ ký/hết hạn/sai audience (FR-AUTH-003, mục 10.2).
/// </summary>
public sealed class BadRequestException(string message) : Exception(message);
