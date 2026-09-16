using FluentValidation;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Ingredients;

public class AddRecipeIngredientCommandValidator : AbstractValidator<AddRecipeIngredientCommand>
{
    public AddRecipeIngredientCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Quantity).GreaterThan(0).When(x => x.Quantity.HasValue);
        RuleFor(x => x.Unit).MaximumLength(50);
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
