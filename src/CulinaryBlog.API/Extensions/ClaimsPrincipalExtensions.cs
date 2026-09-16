using System.Security.Claims;
using CulinaryBlog.Application.Common.Exceptions;

namespace CulinaryBlog.API.Extensions;

/// <summary>
/// Gom việc đọc user id từ JWT về một chỗ. Trước đây mỗi endpoint tự viết
/// <c>user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub")</c> —
/// lặp lại và dễ quên nhánh "sub".
///
/// Vì sao phải thử cả hai: JwtBearer mặc định map claim "sub" của token sang
/// ClaimTypes.NameIdentifier, nhưng nếu ai đó đặt MapInboundClaims = false thì claim giữ
/// nguyên tên "sub". Thử cả hai để không phụ thuộc cấu hình.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>Id user đang đăng nhập. Ném 401 nếu token không có claim định danh.</summary>
    public static string GetUserId(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? user.FindFirstValue("sub")
        ?? throw new UnauthorizedException("Token không chứa thông tin người dùng.");

    /// <summary>Id user nếu đã đăng nhập, null nếu là khách — dùng cho endpoint công khai.</summary>
    public static string? GetUserIdOrNull(this ClaimsPrincipal user) =>
        user.Identity?.IsAuthenticated == true
            ? user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub")
            : null;

    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole("Admin");
}
