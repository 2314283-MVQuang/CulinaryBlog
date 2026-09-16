using FluentValidation;

namespace CulinaryBlog.Application.Features.Recipes.Commands.ChangeStatus;

public class ChangeRecipeStatusCommandValidator : AbstractValidator<ChangeRecipeStatusCommand>
{
    public ChangeRecipeStatusCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.TargetStatus).IsInEnum();
    }
}
