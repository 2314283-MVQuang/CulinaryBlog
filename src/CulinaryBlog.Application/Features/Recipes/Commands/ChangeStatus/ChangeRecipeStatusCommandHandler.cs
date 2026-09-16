using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Features.Recipes.Common;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Enums;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.ChangeStatus;

/// <summary>
/// FR-RCP-005/006. Quy tắc nghiệp vụ:
///
///   - Publish   : bắt buộc công thức phải có ít nhất 1 bước thực hiện (nếu không → 422).
///                 PublishedAt chỉ set MỘT LẦN ở lần publish đầu tiên; publish lại sau khi
///                 unpublish vẫn giữ mốc thời gian cũ để không nhảy lung tung trên trang chủ
///                 (đang sắp xếp theo ngày đăng).
///   - Unpublish : Published → Draft. Giữ nguyên PublishedAt như lịch sử.
///   - Archive   : ẩn khỏi danh sách công khai nhưng KHÔNG xoá dữ liệu (khác hẳn
///                 FR-RCP-007 DELETE là xoá thật).
///
/// Idempotent: gọi publish cho công thức đã Published thì trả về 200 với dữ liệu hiện tại chứ
/// không báo lỗi — client bấm nhầm hai lần hoặc mạng chập chờn gửi lại request vẫn an toàn.
/// </summary>
public class ChangeRecipeStatusCommandHandler(
    IRecipeRepository recipeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ChangeRecipeStatusCommand, RecipeDto>
{
    public async Task<RecipeDto> Handle(ChangeRecipeStatusCommand request, CancellationToken ct)
    {
        var recipe = await recipeRepository.GetByIdWithDetailsAsync(request.RecipeId, ct)
            ?? throw new NotFoundException(nameof(Recipe), request.RecipeId);

        RecipeAuthorization.EnsureCanModify(recipe, request.UserId, request.IsAdmin);

        if (recipe.Status == request.TargetStatus)
        {
            return RecipeMapper.ToDto(recipe);
        }

        if (request.TargetStatus == RecipeStatus.Published)
        {
            EnsureReadyToPublish(recipe);
            recipe.PublishedAt ??= DateTimeOffset.UtcNow;
        }

        recipe.Status = request.TargetStatus;

        // recipe đang được EF Core theo dõi (tracked) nên chỉ cần gán rồi lưu, không cần gọi Update().
        await unitOfWork.SaveChangesAsync(ct);

        return RecipeMapper.ToDto(recipe);
    }

    /// <summary>
    /// Không cho công khai một công thức rỗng ruột. Ném ValidationException để
    /// GlobalExceptionMiddleware trả 422 kèm field "steps" — frontend hiển thị được lỗi
    /// ngay dưới phần nhập các bước.
    /// </summary>
    private static void EnsureReadyToPublish(Recipe recipe)
    {
        var errors = new Dictionary<string, string[]>();

        if (recipe.Steps.Count == 0)
        {
            errors["steps"] = ["Công thức phải có ít nhất 1 bước thực hiện trước khi công khai."];
        }

        if (recipe.Ingredients.Count == 0)
        {
            errors["ingredients"] = ["Công thức phải có ít nhất 1 nguyên liệu trước khi công khai."];
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
    }
}
