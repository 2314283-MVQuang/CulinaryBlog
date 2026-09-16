using FluentValidation;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Images;

/// <summary>
/// CONS-007: chỉ nhận ảnh, tối đa 5MB.
///
/// TODO (nhóm làm tiếp): đây mới là kiểm tra phần "khai báo" (Content-Type + dung lượng do client
/// gửi lên) nên vẫn giả mạo được. Khi làm MinioFileStorageService, kiểm tra thêm magic bytes của
/// file thật (vài byte đầu: JPEG bắt đầu FF D8 FF, PNG 89 50 4E 47...) trước khi lưu.
/// </summary>
public class UploadRecipeImageCommandValidator : AbstractValidator<UploadRecipeImageCommand>
{
    /// <summary>5MB.</summary>
    public const long MaxSizeInBytes = 5 * 1024 * 1024;

    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp",
    ];

    public UploadRecipeImageCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("Chưa chọn file ảnh.")
            .MaximumLength(255);

        RuleFor(x => x.ContentType)
            .Must(type => AllowedContentTypes.Contains(type, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Chỉ chấp nhận ảnh JPEG, PNG hoặc WebP.");

        RuleFor(x => x.SizeInBytes)
            .GreaterThan(0).WithMessage("File ảnh rỗng.")
            .LessThanOrEqualTo(MaxSizeInBytes).WithMessage("Ảnh không được vượt quá 5MB.");

        RuleFor(x => x.AltText).MaximumLength(200);
    }
}
