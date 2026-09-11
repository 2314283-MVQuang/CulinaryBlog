using Microsoft.AspNetCore.Identity;

namespace CulinaryBlog.Domain.Entities;

/// <summary>
/// Người dùng hệ thống, kế thừa IdentityUser (bảng "AspNetUsers" — mục 7.7).
/// Các cột Identity chuẩn (Email, PasswordHash, LockoutEnd...) đã có sẵn từ lớp cha,
/// dưới đây chỉ là các cột CULINARY BLOG thêm vào.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public string? Bio { get; set; }

    /// <summary>Admin có thể deactivate (ban) tài khoản mà không cần xóa dữ liệu.</summary>
    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Recipe> Recipes { get; set; } = [];

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
