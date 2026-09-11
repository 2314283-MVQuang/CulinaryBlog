namespace CulinaryBlog.Application.Common.Interfaces;

/// <summary>
/// FR-JOB-001: Welcome Email Job. TODO (nhóm làm tiếp): bản hiện tại gọi service này TRỰC TIẾP
/// (đồng bộ) trong RegisterCommandHandler để scaffold chạy được không cần Hangfire. Khi tích hợp
/// Hangfire, đổi lời gọi ở RegisterCommandHandler thành
/// <c>BackgroundJob.Enqueue(() => emailService.SendWelcomeEmailAsync(...))</c> (fire-and-forget).
/// </summary>
public interface IEmailService
{
    Task SendWelcomeEmailAsync(string toEmail, string displayName, CancellationToken ct = default);
}
