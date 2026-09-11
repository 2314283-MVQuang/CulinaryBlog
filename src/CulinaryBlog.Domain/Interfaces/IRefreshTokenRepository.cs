using CulinaryBlog.Domain.Entities;

namespace CulinaryBlog.Domain.Interfaces;

/// <summary>
/// RefreshToken không kế thừa BaseEntity nên có repository riêng thay vì dùng IRepository&lt;T&gt;.
/// </summary>
public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken ct = default);

    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);

    /// <summary>Dùng cho Reuse Detection (FR-AUTH-004): thu hồi toàn bộ token còn hiệu lực của user
    /// khi phát hiện có refresh token đã bị revoke mà vẫn được dùng lại ("paranoid mode").</summary>
    Task RevokeAllActiveTokensForUserAsync(string userId, CancellationToken ct = default);

    void Update(RefreshToken token);
}
