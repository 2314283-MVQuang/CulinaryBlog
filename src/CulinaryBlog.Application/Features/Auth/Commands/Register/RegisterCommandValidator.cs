using FluentValidation;

namespace CulinaryBlog.Application.Features.Auth.Commands.Register;

/// <summary>
/// Rule lấy nguyên văn FR-AUTH-001: password tối thiểu 8 ký tự (1 hoa, 1 số, 1 ký tự đặc biệt),
/// userName không chứa ký tự đặc biệt. Đây là ví dụ mẫu cho FluentValidation + MediatR Pipeline
/// (CONS-008) — các Command khác trong dự án viết Validator theo đúng khuôn này.
/// </summary>
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ tên không được để trống.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Email không đúng định dạng.");

        RuleFor(x => x.UserName)
            .NotEmpty()
            .Matches("^[a-zA-Z0-9_.]+$").WithMessage("Tên đăng nhập chỉ được chứa chữ, số, dấu gạch dưới và dấu chấm.")
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự.")
            .Matches("[A-Z]").WithMessage("Mật khẩu phải có ít nhất 1 chữ hoa.")
            .Matches("[0-9]").WithMessage("Mật khẩu phải có ít nhất 1 chữ số.")
            .Matches(@"[!@#$%^&*(),.?"":{}|<>_\-+=\[\]/\\;'~`]").WithMessage("Mật khẩu phải có ít nhất 1 ký tự đặc biệt.");
    }
}
