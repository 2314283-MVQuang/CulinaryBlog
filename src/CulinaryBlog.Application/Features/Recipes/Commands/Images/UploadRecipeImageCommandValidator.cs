using FluentValidation;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Images;

/// <summary>
/// CONS-007: chỉ nhận ảnh, tối đa 5MB.
///
/// Đây mới là phần client KHAI BÁO (Content-Type + dung lượng) nên tự nó giả mạo được. Phần đối
/// chiếu magic bytes của file thật (FR-RCP-008 bước 4) nằm ở UploadRecipeImageCommandHandler, vì
/// chỉ Handler mới chạm được vào Stream.
///
/// TODO (nhóm thống nhất): SRS mục A1–A3 của FR-RCP-008 ghi 400 Bad Request cho lỗi file, nhưng
/// ValidationException của dự án luôn ra 422. Chọn một rồi sửa cả ba chỗ cho đồng nhất.
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
        "image/avif",
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
            .WithMessage("Chỉ chấp nhận ảnh JPEG, PNG, WebP hoặc AVIF.");

        RuleFor(x => x.SizeInBytes)
            .GreaterThan(0).WithMessage("File ảnh rỗng.")
            .LessThanOrEqualTo(MaxSizeInBytes).WithMessage("Ảnh không được vượt quá 5MB.");

        RuleFor(x => x.AltText).MaximumLength(200);
    }
}
