namespace CulinaryBlog.Infrastructure.Services;

/// <summary>Đọc từ section "Jwt" trong appsettings (mục 5.2: access token HS256, TTL 15 phút).
/// Secret PHẢI đặt qua User Secrets (dev) / biến môi trường (prod), KHÔNG commit vào Git (CONS: mục 5.2).</summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = null!;

    public string Issuer { get; set; } = "CulinaryBlog";

    public string Audience { get; set; } = "CulinaryBlog";

    public int AccessTokenMinutes { get; set; } = 15;

    public int RefreshTokenDays { get; set; } = 7;
}
