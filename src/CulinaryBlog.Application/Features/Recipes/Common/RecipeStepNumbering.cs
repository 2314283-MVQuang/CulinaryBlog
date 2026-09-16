using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Interfaces;

namespace CulinaryBlog.Application.Features.Recipes.Common;

/// <summary>
/// Đánh số lại các bước thành 1..n theo đúng thứ tự truyền vào (FR-RCP-010).
///
/// VÌ SAO PHẢI LƯU HAI LẦN — đọc kỹ trước khi "tối ưu" xuống còn một lần:
/// Bảng "RecipeSteps" có ràng buộc UNIQUE("RecipeId", "StepNumber"). PostgreSQL kiểm tra ràng
/// buộc này ngay sau TỪNG câu UPDATE chứ không đợi hết transaction. Nếu đổi trực tiếp
/// bước 3 thành 2 trong khi bước 2 cũ vẫn còn, câu UPDATE đó lỗi ngay lập tức.
///
/// Cách làm an toàn tuyệt đối, không phụ thuộc thứ tự EF Core sinh câu lệnh:
///   Pha 1 — dời tất cả lên vùng số CAO HƠN mọi số đang có (max+1, max+2, ...) rồi lưu.
///           Không số nào đụng số nào vì vùng này hoàn toàn trống.
///   Pha 2 — hạ về 1..n rồi lưu. Lúc này vùng 1..n đã trống sạch.
/// Dùng số dương ở pha 1 (không dùng số âm) vì DB còn CHECK("StepNumber" > 0).
/// </summary>
public static class RecipeStepNumbering
{
    public static async Task ApplyOrderAsync(
        IReadOnlyList<RecipeStep> desiredOrder,
        IUnitOfWork unitOfWork,
        CancellationToken ct)
    {
        if (desiredOrder.Count == 0 || IsAlreadyInOrder(desiredOrder))
        {
            return;
        }

        var offset = desiredOrder.Max(s => s.StepNumber);

        // Hai pha nằm trong CÙNG một transaction: lỡ có sự cố giữa chừng thì DB quay về trạng
        // thái cũ, không để lại các bước mang số tạm (max+1, max+2...) mà người dùng nhìn thấy.
        await unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                for (var i = 0; i < desiredOrder.Count; i++)
                {
                    desiredOrder[i].StepNumber = offset + 1 + i;
                }

                await unitOfWork.SaveChangesAsync(ct);

                for (var i = 0; i < desiredOrder.Count; i++)
                {
                    desiredOrder[i].StepNumber = i + 1;
                }

                await unitOfWork.SaveChangesAsync(ct);
            },
            ct);
    }

    /// <summary>Đã đúng 1..n theo thứ tự mong muốn thì khỏi đụng DB.</summary>
    private static bool IsAlreadyInOrder(IReadOnlyList<RecipeStep> desiredOrder)
    {
        for (var i = 0; i < desiredOrder.Count; i++)
        {
            if (desiredOrder[i].StepNumber != i + 1)
            {
                return false;
            }
        }

        return true;
    }
}
