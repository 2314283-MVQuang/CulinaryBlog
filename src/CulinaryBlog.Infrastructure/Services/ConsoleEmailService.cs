using CulinaryBlog.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CulinaryBlog.Infrastructure.Services;

/// <summary>
/// TODO (nhóm làm tiếp): implementation TẠM THỜI của IEmailService — chỉ ghi log thay vì gửi mail
/// thật, để scaffold chạy được ngay không cần cấu hình SMTP. Khi triển khai FR-JOB-001 thật, viết
/// MailKitEmailService (SMTP dev qua MailHog, SendGrid production — mục 3.1) implement interface
/// NÀY, rồi đổi đăng ký DI ở Infrastructure/DependencyInjection.cs.
/// </summary>
public class ConsoleEmailService(ILogger<ConsoleEmailService> logger) : IEmailService
{
    public Task SendWelcomeEmailAsync(string toEmail, string displayName, CancellationToken ct = default)
    {
        logger.LogInformation(
            "[EMAIL GIẢ LẬP] Gửi email chào mừng tới {Email} (tên hiển thị: {DisplayName})",
            toEmail, displayName);

        return Task.CompletedTask;
    }
}
