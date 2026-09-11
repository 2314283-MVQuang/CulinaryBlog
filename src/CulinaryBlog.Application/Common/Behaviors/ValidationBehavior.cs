using FluentValidation;
using MediatR;
using ValidationException = CulinaryBlog.Application.Common.Exceptions.ValidationException;

namespace CulinaryBlog.Application.Common.Behaviors;

/// <summary>
/// Pipeline Behavior #2 (mục 6.3, CONS-008): chạy mọi IValidator&lt;TRequest&gt; đã đăng ký
/// cho request này TRƯỚC KHI vào Handler. Không có validator nào cho request thì bỏ qua.
/// Đây là lý do FluentValidation KHÔNG được gọi trực tiếp trong Endpoint hay Handler.
/// </summary>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, ct))))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
