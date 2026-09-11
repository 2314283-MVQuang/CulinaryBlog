namespace CulinaryBlog.Application.Common.Exceptions;

/// <summary>Map sang 423 Locked. Sau 5 lần sai mật khẩu, tài khoản khóa 15 phút (FR-AUTH-002).</summary>
public sealed class LockedOutException(string message) : Exception(message);
