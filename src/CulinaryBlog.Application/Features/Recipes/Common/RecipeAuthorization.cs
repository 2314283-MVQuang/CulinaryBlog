using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Domain.Entities;

namespace CulinaryBlog.Application.Features.Recipes.Common;

/// <summary>
/// Resource-Based Authorization cho Recipe (mục 2.1): role "Author" chỉ cho phép sửa công thức
/// DO CHÍNH MÌNH tạo, còn Admin sửa được của mọi người.
///
/// Đặt ở Application Layer (không phải AuthorizationHandler của ASP.NET) vì quyền này phụ thuộc
/// dữ liệu đã tải từ DB — Handler nào cũng phải gọi, nên gom về một chỗ để không ai quên.
/// </summary>
public static class RecipeAuthorization
{
    public static void EnsureCanModify(Recipe recipe, string userId, bool isAdmin)
    {
        if (isAdmin || string.Equals(recipe.AuthorId, userId, StringComparison.Ordinal))
        {
            return;
        }

        throw new ForbiddenAccessException("Bạn chỉ có thể chỉnh sửa công thức do mình tạo.");
    }
}
