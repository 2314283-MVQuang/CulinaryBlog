namespace CulinaryBlog.Application.Common.Interfaces;

/// <summary>
/// Bọc lại thông tin user hiện tại từ JWT claims (HttpContext.User), để Application Layer
/// không phụ thuộc trực tiếp vào ASP.NET Core (ClaimsPrincipal). Implement ở Presentation/API
/// bằng IHttpContextAccessor.
/// </summary>
public interface ICurrentUser
{
    string? UserId { get; }

    string? Email { get; }

    bool IsAuthenticated { get; }

    bool IsInRole(string role);
}
