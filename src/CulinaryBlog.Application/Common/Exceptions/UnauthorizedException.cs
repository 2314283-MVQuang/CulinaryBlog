namespace CulinaryBlog.Application.Common.Exceptions;

/// <summary>Map sang 401 Unauthorized. Luôn dùng message CHUNG CHUNG khi sai email/password
/// để chống User Enumeration Attack (FR-AUTH-002, mục 5.2) — không bao giờ nói rõ "email không tồn tại".</summary>
public sealed class UnauthorizedException(string message) : Exception(message);
