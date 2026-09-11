namespace CulinaryBlog.Application.Features.Auth.Dtos;

/// <summary>
/// FR-AUTH-006: KHÔNG BAO GIỜ chứa PasswordHash/SecurityStamp.
/// Roles kiểu IList&lt;string&gt; để khớp trực tiếp với kiểu trả về của UserManager.GetRolesAsync()
/// (IList&lt;string&gt; KHÔNG kế thừa IReadOnlyList&lt;string&gt; trong .NET, nên dùng IReadOnlyList
/// ở đây sẽ bắt buộc gọi .ToList() thừa thãi ở mọi Handler).
/// </summary>
public record UserProfileDto(
    string Id,
    string Email,
    string DisplayName,
    string? AvatarUrl,
    string? Bio,
    IList<string> Roles);
