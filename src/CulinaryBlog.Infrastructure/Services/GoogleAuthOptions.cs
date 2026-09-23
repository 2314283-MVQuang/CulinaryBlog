namespace CulinaryBlog.Infrastructure.Services;

/// <summary>
/// appsettings.json → section "Google" (mục 6.2). Client ID KHÔNG phải secret (đây là public OAuth
/// Client ID, an toàn khi commit) nên để thẳng trong appsettings.json — khác với Jwt:Secret phải
/// nằm trong user-secrets.
/// </summary>
public class GoogleAuthOptions
{
    public const string SectionName = "Google";

    public string ClientId { get; set; } = string.Empty;
}
