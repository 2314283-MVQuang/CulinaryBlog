namespace CulinaryBlog.Application.Common.Interfaces;

/// <summary>
/// Verify idToken do Google Sign-In JS SDK phía client gửi lên (FR-AUTH-003, mục 5.2).
/// Implementation nằm ở Infrastructure Layer (gói Google.Apis.Auth) — Application Layer chỉ biết
/// interface này, không phụ thuộc trực tiếp vào thư viện Google (CONS-003: Clean Architecture).
/// </summary>
public interface IGoogleAuthService
{
    /// <summary>
    /// Verify chữ ký + hạn dùng + audience (Client ID) của idToken với Google.
    /// Ném BadRequestException("AUTH_GOOGLE_TOKEN_INVALID"...) nếu token sai/hết hạn — Handler
    /// không cần tự bắt lỗi thư viện Google.
    /// </summary>
    Task<GoogleUserInfo> VerifyIdTokenAsync(string idToken, CancellationToken ct = default);
}

/// <summary>Thông tin lấy được từ idToken đã verify — chỉ những trường FR-AUTH-003 cần dùng.</summary>
public record GoogleUserInfo(string Email, string Name, string? AvatarUrl, bool EmailVerified);
