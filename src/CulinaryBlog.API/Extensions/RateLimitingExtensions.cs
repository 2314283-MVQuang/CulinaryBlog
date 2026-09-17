using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CulinaryBlog.API.Extensions;

/// <summary>
/// FR-OBS-003: Rate limiting cho các endpoint xác thực (đăng nhập / đăng ký).
/// Chống tấn công dò mật khẩu (brute-force) và spam tạo tài khoản rác.
/// Trả về mã lỗi 429 Too Many Requests theo chuẩn RFC 7807 (CONS-005).
/// </summary>
public static class RateLimitingExtensions
{
    public const string AuthPolicyName = "AuthRateLimit";

    public static IServiceCollection AddAuthRateLimiter(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("RateLimiting:Auth");
        var permitLimit = section.GetValue<int?>("PermitLimit") ?? 5; // Mặc định 5 request
        var windowSeconds = section.GetValue<int?>("WindowSeconds") ?? 60; // Trong 60 giây
        var queueLimit = section.GetValue<int?>("QueueLimit") ?? 0; // Không xếp hàng chờ

        services.AddRateLimiter(options =>
        {
            // Trả về RFC 7807 Problem Details khi bị vượt quá giới hạn (CONS-005)
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/problem+json";

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    var retrySeconds = Math.Max(1, (int)retryAfter.TotalSeconds);
                    context.HttpContext.Response.Headers.RetryAfter = retrySeconds.ToString();
                }

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status429TooManyRequests,
                    Title = "Quá nhiều yêu cầu",
                    Detail = "Bạn đã gửi quá nhiều yêu cầu đăng nhập/đăng ký trong thời gian ngắn. Vui lòng thử lại sau.",
                    Instance = context.HttpContext.Request.Path,
                    Type = "https://tools.ietf.org/html/rfc6585#section-4"
                };

                problemDetails.Extensions["code"] = "AUTH_RATE_LIMITED";

                await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
            };

            // Phân vùng rate limit theo địa chỉ IP của client
            options.AddPolicy(AuthPolicyName, httpContext =>
            {
                var clientIp = httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? "unknown-client";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: clientIp,
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = TimeSpan.FromSeconds(windowSeconds),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = queueLimit,
                        AutoReplenishment = true
                    });
            });
        });

        return services;
    }
}
