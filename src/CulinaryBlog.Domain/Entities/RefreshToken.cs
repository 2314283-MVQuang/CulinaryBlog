namespace CulinaryBlog.Domain.Entities;

/// <summary>
/// Refresh token (bảng "RefreshTokens" — mục 7.8). KHÔNG kế thừa BaseEntity vì không cần
/// soft-delete/RowVersion, chỉ cần các cột audit riêng của nó.
/// CHỈ lưu SHA-256 hash của token, không bao giờ lưu token gốc (bảo mật — mục 5.2).
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;

    public string TokenHash { get; set; } = null!;

    /// <summary>7 ngày kể từ CreatedAt.</summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>NULL = còn hiệu lực.</summary>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>Hash của token mới khi rotation (FR-AUTH-004) — dùng để trace "token family" khi phát hiện reuse.</summary>
    public string? ReplacedByTokenHash { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public string? CreatedByIp { get; set; }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTimeOffset.UtcNow;
}
