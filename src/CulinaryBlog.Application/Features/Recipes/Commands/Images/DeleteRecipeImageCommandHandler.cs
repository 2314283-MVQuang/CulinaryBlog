using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Features.Recipes.Common;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Images;

public class DeleteRecipeImageCommandHandler(
    IRecipeRepository recipeRepository,
    IRepository<RecipeImage> imageRepository,
    IFileStorageService fileStorage,
    IUnitOfWork unitOfWork,
    ILogger<DeleteRecipeImageCommandHandler> logger)
    : IRequestHandler<DeleteRecipeImageCommand>
{
    public async Task Handle(DeleteRecipeImageCommand request, CancellationToken ct)
    {
        var recipe = await recipeRepository.GetByIdWithDetailsAsync(request.RecipeId, ct)
            ?? throw new NotFoundException(nameof(Recipe), request.RecipeId);

        RecipeAuthorization.EnsureCanModify(recipe, request.UserId, request.IsAdmin);

        var image = recipe.Images.FirstOrDefault(i => i.Id == request.ImageId)
            ?? throw new NotFoundException(nameof(RecipeImage), request.ImageId);

        var wasPrimary = image.IsPrimary;
        var filesToDelete = new[] { image.OriginalUrl, image.MediumUrl, image.ThumbnailUrl };

        // Xoá dòng ảnh và đôn ảnh khác lên làm đại diện phải là HAI lần lưu (sau khi dòng cũ đã
        // biến mất mới được bật IsPrimary cho ảnh khác, nếu không đụng partial unique index
        // "UQ_RecipeImages_OnePrimaryPerRecipe"), nhưng nằm chung một transaction để không bao giờ
        // rơi vào tình trạng "đã xoá ảnh đại diện mà chưa kịp chọn ảnh thay thế".
        await unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                recipe.Images.Remove(image);
                imageRepository.Remove(image);
                await unitOfWork.SaveChangesAsync(ct);

                if (wasPrimary && recipe.Images.Count > 0)
                {
                    recipe.Images.OrderBy(i => i.OrderIndex).First().IsPrimary = true;
                    await unitOfWork.SaveChangesAsync(ct);
                }
            },
            ct);

        // Xoá file SAU KHI DB đã commit: nếu làm ngược lại mà SaveChanges lỗi thì DB còn dòng
        // trỏ tới file không còn tồn tại. DeleteAsync là idempotent (file không có thì bỏ qua).
        //
        // Bọc try/catch vì tới đây DB đã commit xong: kho lưu trữ trục trặc thì để lại file mồ côi
        // (dọn sau bằng job quét rác) chứ không ném lỗi 500 làm client tưởng xoá thất bại.
        foreach (var url in filesToDelete)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                continue;
            }

            try
            {
                await fileStorage.DeleteAsync(url, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    "Đã xoá ảnh {ImageId} khỏi DB nhưng không xoá được file {FileUrl}.",
                    request.ImageId,
                    url);
            }
        }
    }
}
