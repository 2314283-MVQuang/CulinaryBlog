using CulinaryBlog.Domain.Entities;

namespace CulinaryBlog.Application.Common.Interfaces;

/// <summary>Sinh access token (JWT HS256, 15 phút) và refresh token (random 512-bit, mục 5.2).</summary>
public interface IJwtService
{
    /// <summary>Claims bắt buộc: userId, email, roles, jti (mục 5.2).</summary>
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);

    /// <summary>Trả về cặp (token gốc để gửi cho client, hash SHA-256 để lưu DB).
    /// Không bao giờ lưu token gốc — xem RefreshToken.TokenHash.</summary>
    (string RawToken, string TokenHash) GenerateRefreshToken();

    /// <summary>Hash 1 refresh token do client gửi lên để so sánh với TokenHash trong DB.</summary>
    string HashRefreshToken(string rawToken);

    /// <summary>Thời gian sống access token, tính bằng giây — dùng để trả về AuthResponseDto.ExpiresIn.</summary>
    int AccessTokenLifetimeSeconds { get; }
}
