using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Features.Recipes.Common;
using CulinaryBlog.Application.Features.Recipes.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.Images;

public class UploadRecipeImageCommandHandler(
    IRecipeRepository recipeRepository,
    IFileStorageService fileStorage,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UploadRecipeImageCommand, RecipeImageDto>
{
    public async Task<RecipeImageDto> Handle(UploadRecipeImageCommand request, CancellationToken ct)
    {
        var recipe = await recipeRepository.GetByIdWithDetailsAsync(request.RecipeId, ct)
            ?? throw new NotFoundException(nameof(Recipe), request.RecipeId);

        // Kiểm tra quyền TRƯỚC khi ghi file: tránh việc người không có quyền vẫn kịp đẩy file rác lên đĩa.
        RecipeAuthorization.EnsureCanModify(recipe, request.UserId, request.IsAdmin);

        var originalUrl = await fileStorage.UploadAsync(
            request.Content,
            request.FileName,
            request.ContentType,
            $"recipes/{recipe.Id}",
            ct);

        var image = new RecipeImage
        {
            RecipeId = recipe.Id,
            OriginalUrl = originalUrl,
            AltText = string.IsNullOrWhiteSpace(request.AltText) ? null : request.AltText.Trim(),
            OrderIndex = recipe.Images.Count == 0 ? 0 : recipe.Images.Max(i => i.OrderIndex) + 1,
            // Ảnh đầu tiên mặc định là ảnh đại diện. Không sợ đụng partial unique index
            // "UQ_RecipeImages_OnePrimaryPerRecipe" vì chỉ set true khi chưa có ảnh nào là primary.
            IsPrimary = !recipe.Images.Any(i => i.IsPrimary),
        };

        // MediumUrl/ThumbnailUrl để null — sẽ do background job resize sinh ra (FR-JOB-002,
        // TODO của bạn Bảo Thinh khi tích hợp Hangfire).
        recipe.Images.Add(image);
        await unitOfWork.SaveChangesAsync(ct);

        return RecipeMapper.ToDto(image);
    }
}
