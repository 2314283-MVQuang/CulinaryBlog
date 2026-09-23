namespace CulinaryBlog.Application.Features.Recipes.Dtos;

/// <summary>Một bước thực hiện (mục 5.3, FR-RCP-010).</summary>
public record RecipeStepDto(
    Guid Id,
    int StepNumber,
    string Title,
    string Description,
    int? TimerMinutes,
    string? ImageUrl);
