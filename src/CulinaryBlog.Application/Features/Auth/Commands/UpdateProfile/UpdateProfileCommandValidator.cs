using FluentValidation;

namespace CulinaryBlog.Application.Features.Auth.Commands.UpdateProfile;

/// <summary>
/// Chỉ validate field nào client THỰC SỰ gửi lên (khác null) — đó là ý nghĩa của PATCH.
/// Vì vậy mọi rule đều bọc trong .When(... is not null).
/// </summary>
public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        // DisplayName có gửi lên thì không được để trống (khác với "không gửi" = giữ nguyên).
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Tên hiển thị không được để trống.")
            .MaximumLength(100)
            .When(x => x.DisplayName is not null);

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(500)
            .Must(BeAbsoluteOrRelativeUrl).WithMessage("Đường dẫn ảnh đại diện không hợp lệ.")
            .When(x => !string.IsNullOrEmpty(x.AvatarUrl));

        RuleFor(x => x.Bio)
            .MaximumLength(1000)
            .When(x => x.Bio is not null);
    }

    /// <summary>
    /// Chấp nhận cả URL tuyệt đối ("https://...") lẫn đường dẫn tương đối do chính API sinh ra
    /// khi upload ảnh ("/uploads/avatars/abc.jpg" — xem IFileStorageService).
    /// </summary>
    private static bool BeAbsoluteOrRelativeUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        if (value.StartsWith('/'))
        {
            return true;
        }

        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
