using FluentValidation;

namespace CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;

/// <summary>Rule lấy nguyên văn mục 5.2 (Recipe): PrepTime > 0, CookTime >= 0, Servings > 0, Description ≤2000 ký tự.</summary>
public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tiêu đề không được để trống.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Mô tả không được để trống.")
            .MaximumLength(2000);

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Phải chọn danh mục.");

        RuleFor(x => x.PrepTime)
            .GreaterThan(0).WithMessage("Thời gian chuẩn bị phải lớn hơn 0 phút.");

        RuleFor(x => x.CookTime)
            .GreaterThanOrEqualTo(0).WithMessage("Thời gian nấu phải lớn hơn hoặc bằng 0 phút.");

        RuleFor(x => x.Servings)
            .GreaterThan(0).WithMessage("Số khẩu phần phải lớn hơn 0.");

        RuleFor(x => x.Difficulty)
            .IsInEnum().WithMessage("Độ khó không hợp lệ.");
    }
}
