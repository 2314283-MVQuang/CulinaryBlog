using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Common.Helpers;
using CulinaryBlog.Application.Features.Categories.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Commands.CreateCategory;

/// <summary>
/// FR-CAT-003. Slug tự sinh (slugify), nếu trùng thì thêm hậu tố số.
/// TODO (nhóm làm tiếp): invalidate cache "categories:all" sau khi tạo (CacheInvalidationBehavior,
/// mục 6.3) khi tích hợp Redis.
/// </summary>
public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        if (await categoryRepository.NameExistsAsync(request.Name, ct))
        {
            throw new ConflictException($"Danh mục \"{request.Name}\" đã tồn tại.");
        }

        var slug = await GenerateUniqueSlugAsync(request.Name, ct);

        var category = new Category
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            OrderIndex = request.OrderIndex,
        };

        await categoryRepository.AddAsync(category, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new CategoryDto(category.Id, category.Name, category.Slug, category.Description, category.ImageUrl, 0);
    }

    private async Task<string> GenerateUniqueSlugAsync(string name, CancellationToken ct)
    {
        var baseSlug = SlugHelper.GenerateSlug(name);
        var slug = baseSlug;
        var suffix = 2;

        while (await categoryRepository.SlugExistsAsync(slug, ct))
        {
            slug = SlugHelper.AppendSuffix(baseSlug, suffix++);
        }

        return slug;
    }
}
