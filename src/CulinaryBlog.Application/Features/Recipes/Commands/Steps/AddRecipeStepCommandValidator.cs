using FluentValidation;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Steps;

public class AddRecipeStepCommandValidator : AbstractValidator<AddRecipeStepCommand>
{
    public AddRecipeStepCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        // DB chấp nhận 0 (CHECK "TimerMinutes" >= 0) — bước không cần hẹn giờ thì để null hoặc 0.
        RuleFor(x => x.TimerMinutes).GreaterThanOrEqualTo(0).When(x => x.TimerMinutes.HasValue);
        RuleFor(x => x.ImageUrl).MaximumLength(500);
    }
}
