using FluentValidation;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipes;

public class GetRecipesQueryValidator : AbstractValidator<GetRecipesQuery>
{
    public GetRecipesQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50); // max 50 (mục 4.4)
        RuleFor(x => x.MaxCookTime).GreaterThanOrEqualTo(0).When(x => x.MaxCookTime.HasValue);
    }
}
