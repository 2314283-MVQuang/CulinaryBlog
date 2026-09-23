using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Enums;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;

/// <summary>
/// FR-RCP-003 — POST /api/v1/recipes (Actor: Author/Admin). Tạo với Status=Draft. Nutrition tùy
/// chọn. AuthorId do Endpoint lấy từ JWT claims truyền vào, KHÔNG nhận từ body (chống tạo hộ
/// người khác) — giống cách RegisterCommand/UpdateProfileCommand không tự đọc ClaimsPrincipal.
/// </summary>
public record CreateRecipeCommand(
    string AuthorId,
    string Title,
    string Description,
    Guid CategoryId,
    int PrepTime,
    int CookTime,
    int Servings,
    RecipeDifficulty Difficulty,
    string? Instructions,
    RecipeNutritionDto? Nutrition) : IRequest<RecipeDetailDto>;
