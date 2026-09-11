namespace CulinaryBlog.Application.Common.Exceptions;

/// <summary>
/// Map sang 403 Forbidden. Ném ra khi Resource-Based Authorization thất bại (mục 2.1) —
/// ví dụ Author cố sửa recipe không phải của mình.
/// </summary>
public sealed class ForbiddenAccessException(string message = "Bạn không có quyền thực hiện thao tác này.")
    : Exception(message);
