using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Common.Helpers;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Enums;
using CulinaryBlog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;

/// <summary>
/// FR-RCP-003. Slug sinh tự động từ Title, trùng thì tự thêm hậu tố số ("pho-bo", "pho-bo-2"...) —
/// KHÔNG BAO GIỜ trả lỗi vì trùng slug (khác Category, nơi trùng Name mới báo lỗi).
/// </summary>
public class CreateRecipeCommandHandler(
    IRepository<Recipe> recipeRepository,
    IRepository<Category> categoryRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateRecipeCommand, RecipeDetailDto>
{
    public async Task<RecipeDetailDto> Handle(CreateRecipeCommand request, CancellationToken ct)
    {
        _ = await categoryRepository.GetByIdAsync(request.CategoryId, ct)
            ?? throw new NotFoundException(nameof(Category), request.CategoryId);

        var recipe = new Recipe
        {
            Title = request.Title.Trim(),
            Slug = await GenerateUniqueSlugAsync(request.Title, ct),
            Description = request.Description.Trim(),
            // Instructions ở DB hiện là NOT NULL (xem RecipeConfiguration.cs) dù mục 5.2 mô tả là
            // trường tùy chọn — để trống thì lưu chuỗi rỗng thay vì null, tránh vi phạm ràng buộc.
            Instructions = request.Instructions?.Trim() ?? string.Empty,
            PrepTime = request.PrepTime,
            CookTime = request.CookTime,
            Servings = request.Servings,
            Difficulty = request.Difficulty,
            Status = RecipeStatus.Draft,
            CategoryId = request.CategoryId,
            AuthorId = request.AuthorId,
            Nutrition = new RecipeNutrition
            {
                Calories = request.Nutrition?.Calories,
                Protein = request.Nutrition?.Protein,
                Carbohydrates = request.Nutrition?.Carbohydrates,
                Fat = request.Nutrition?.Fat,
                Fiber = request.Nutrition?.Fiber,
                Sodium = request.Nutrition?.Sodium,
            },
        };

        await recipeRepository.AddAsync(recipe, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // Nạp lại kèm Category/Author/Steps/Ingredients/Images để map RecipeDetailDto đủ dữ liệu —
        // cùng cách GetRecipeBySlugQueryHandler làm, tránh phải tự set thủ công từng navigation.
        var created = await recipeRepository.Query()
            .Include(r => r.Category)
            .Include(r => r.Author)
            .Include(r => r.Steps)
            .Include(r => r.Ingredients)
            .Include(r => r.Images)
            .FirstAsync(r => r.Id == recipe.Id, ct);

        return created.ToDetailDto();
    }

    /// <summary>
    /// "pho-bo" đã tồn tại thì thử "pho-bo-2", "pho-bo-3"... tới khi tìm được slug trống. Vòng lặp
    /// luôn dừng vì hậu tố tăng dần vô hạn, không cần giới hạn số lần thử nhân tạo.
    /// </summary>
    private async Task<string> GenerateUniqueSlugAsync(string title, CancellationToken ct)
    {
        var baseSlug = SlugHelper.GenerateSlug(title);
        var slug = baseSlug;
        var suffix = 2;

        while (await recipeRepository.Query().AnyAsync(r => r.Slug == slug, ct))
        {
            slug = SlugHelper.AppendSuffix(baseSlug, suffix);
            suffix++;
        }

        return slug;
    }
}
