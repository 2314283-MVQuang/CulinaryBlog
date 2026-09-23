using CulinaryBlog.Domain.Enums;
using FluentValidation;

namespace CulinaryBlog.Application.Features.Recipes.Queries.ListRecipes;

/// <summary>
/// FR-SRCH-002/003/004: pageSize tối đa 50, sort chỉ nhận field trong whitelist (chặn truyền field
/// bất kỳ để tránh lộ tên cột nội bộ hoặc gây lỗi khi build IQueryable ở Handler).
/// </summary>
public class ListRecipesQueryValidator : AbstractValidator<ListRecipesQuery>
{
    private static readonly string[] AllowedSortFields =
        ["createdAt", "title", "prepTime", "cookTime", "servings", "publishedAt"];

    public ListRecipesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("page phải >= 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("pageSize phải trong khoảng 1-50.");

        RuleFor(x => x.Sort)
            .Must(sort => AllowedSortFields.Contains(sort.TrimStart('-')))
            .WithMessage($"sort chỉ chấp nhận: {string.Join(", ", AllowedSortFields)} (thêm dấu \"-\" ở đầu để sắp giảm dần).");

        RuleFor(x => x.Difficulty)
            .Must(d => Enum.TryParse<RecipeDifficulty>(d, ignoreCase: true, out _))
            .When(x => x.Difficulty is not null)
            .WithMessage("difficulty phải là một trong: Easy, Medium, Hard, Expert.");

        RuleFor(x => x.MaxCookTime)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxCookTime.HasValue)
            .WithMessage("maxCookTime phải >= 0.");

        RuleFor(x => x.MinServings)
            .GreaterThan(0)
            .When(x => x.MinServings.HasValue)
            .WithMessage("minServings phải > 0.");
    }
}
