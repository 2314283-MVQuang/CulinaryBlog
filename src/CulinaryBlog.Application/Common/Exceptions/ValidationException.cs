using FluentValidation.Results;

namespace CulinaryBlog.Application.Common.Exceptions;

/// <summary>
/// Ném ra bởi ValidationBehavior khi FluentValidation phát hiện lỗi. GlobalExceptionMiddleware
/// (ở API layer) bắt exception này và trả về 422 Unprocessable Entity theo RFC 7807 (CONS-005).
/// </summary>
public sealed class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("Một hoặc nhiều trường dữ liệu không hợp lệ.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : this()
    {
        Errors = errors;
    }
}
