namespace CulinaryBlog.Application.Common.Exceptions;

/// <summary>Map sang 404 Not Found ở GlobalExceptionMiddleware.</summary>
public sealed class NotFoundException(string entityName, object key)
    : Exception($"Không tìm thấy {entityName} với khóa \"{key}\".");
