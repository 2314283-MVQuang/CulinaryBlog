using CulinaryBlog.Domain.Entities;

namespace CulinaryBlog.Application.Features.Recipes.Common;

/// <summary>
/// Đánh lại OrderIndex thành 0..n-1 theo đúng thứ tự truyền vào (FR-RCP-009).
///
/// Khác với các bước nấu (RecipeStepNumbering), nguyên liệu KHÔNG có ràng buộc UNIQUE trên
/// (RecipeId, OrderIndex) — xem db/init/02-schema.sql — nên đổi số thoải mái trong một lần
/// SaveChanges, không cần mẹo hai pha. Hàm này chỉ gán giá trị, việc lưu do Handler gọi.
/// </summary>
public static class RecipeIngredientOrdering
{
    public static void ApplyOrder(IReadOnlyList<RecipeIngredient> desiredOrder)
    {
        for (var i = 0; i < desiredOrder.Count; i++)
        {
            desiredOrder[i].OrderIndex = i;
        }
    }
}
