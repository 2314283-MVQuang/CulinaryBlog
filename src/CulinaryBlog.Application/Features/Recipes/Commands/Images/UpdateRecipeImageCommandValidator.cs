using FluentValidation;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Images;

public class UpdateRecipeImageCommandValidator : AbstractValidator<UpdateRecipeImageCommand>
{
    public UpdateRecipeImageCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmpty();
        RuleFor(x => x.ImageId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.AltText).MaximumLength(200);
        RuleFor(x => x.OrderIndex).GreaterThanOrEqualTo(0).When(x => x.OrderIndex.HasValue);
    }
}
