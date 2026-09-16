using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Features.Recipes.Common;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Images;

public class UpdateRecipeImageCommandHandler(
    IRecipeRepository recipeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRecipeImageCommand, RecipeImageDto>
{
    public async Task<RecipeImageDto> Handle(UpdateRecipeImageCommand request, CancellationToken ct)
    {
        var recipe = await recipeRepository.GetByIdWithDetailsAsync(request.RecipeId, ct)
            ?? throw new NotFoundException(nameof(Recipe), request.RecipeId);

        RecipeAuthorization.EnsureCanModify(recipe, request.UserId, request.IsAdmin);

        var image = recipe.Images.FirstOrDefault(i => i.Id == request.ImageId)
            ?? throw new NotFoundException(nameof(RecipeImage), request.ImageId);

        image.AltText = string.IsNullOrWhiteSpace(request.AltText) ? null : request.AltText.Trim();

        if (request.OrderIndex is int target && target != image.OrderIndex)
        {
            var desiredOrder = recipe.Images.OrderBy(i => i.OrderIndex).ToList();
            desiredOrder.Remove(image);

            var index = Math.Clamp(target, 0, desiredOrder.Count);
            desiredOrder.Insert(index, image);

            for (var i = 0; i < desiredOrder.Count; i++)
            {
                desiredOrder[i].OrderIndex = i;
            }
        }

        await unitOfWork.SaveChangesAsync(ct);

        if (request.IsPrimary is bool shouldBePrimary)
        {
            await SwitchPrimaryAsync(recipe, image, shouldBePrimary, ct);
        }

        return RecipeMapper.ToDto(image);
    }

    /// <summary>
    /// Đổi ảnh đại diện phải lưu LÀM HAI LẦN, không gộp được.
    ///
    /// Lý do: DB có partial unique index "UQ_RecipeImages_OnePrimaryPerRecipe" — mỗi công thức
    /// chỉ được đúng một dòng IsPrimary = true. PostgreSQL kiểm tra ngay sau từng câu UPDATE,
    /// nên nếu EF Core tình cờ sinh câu "bật ảnh mới" TRƯỚC câu "tắt ảnh cũ" thì lỗi trùng khoá.
    /// Tắt ảnh cũ + lưu, rồi mới bật ảnh mới + lưu là chắc chắn đúng thứ tự.
    /// </summary>
    private async Task SwitchPrimaryAsync(Recipe recipe, RecipeImage image, bool shouldBePrimary, CancellationToken ct)
    {
        if (shouldBePrimary == image.IsPrimary)
        {
            return;
        }

        if (!shouldBePrimary)
        {
            // Bỏ đánh dấu đại diện: công thức tạm thời không có ảnh đại diện, danh sách sẽ hiển
            // thị ảnh mặc định. Muốn đổi sang ảnh khác thì gọi PUT cho ảnh đó với isPrimary = true.
            image.IsPrimary = false;
            await unitOfWork.SaveChangesAsync(ct);
            return;
        }

        var currentPrimary = recipe.Images.FirstOrDefault(i => i.IsPrimary && i.Id != image.Id);
        if (currentPrimary is null)
        {
            image.IsPrimary = true;
            await unitOfWork.SaveChangesAsync(ct);
            return;
        }

        // Gán sang biến kiểu KHÔNG nullable trước khi đưa vào lambda: trình biên dịch không giữ
        // được kết quả kiểm tra null qua ranh giới lambda nên sẽ cảnh báo CS8602 nếu dùng thẳng.
        RecipeImage previousPrimary = currentPrimary;

        // Tắt ảnh cũ rồi mới bật ảnh mới, cả hai trong cùng transaction để không có khoảnh khắc
        // nào công thức bị mất ảnh đại diện nếu lỡ hỏng giữa chừng.
        await unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                previousPrimary.IsPrimary = false;
                await unitOfWork.SaveChangesAsync(ct);

                image.IsPrimary = true;
                await unitOfWork.SaveChangesAsync(ct);
            },
            ct);
    }
}
