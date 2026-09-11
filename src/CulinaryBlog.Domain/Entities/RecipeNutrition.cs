namespace CulinaryBlog.Domain.Entities;

/// <summary>
/// Thông tin dinh dưỡng — Owned Entity của Recipe (mục 7.2), KHÔNG có bảng riêng,
/// EF Core nhúng thẳng vào bảng "Recipes" với tiền tố cột "Nutrition_". Tất cả nullable,
/// tính trên 1 khẩu phần.
/// </summary>
public class RecipeNutrition
{
    public decimal? Calories { get; set; }

    public decimal? Protein { get; set; }

    public decimal? Carbohydrates { get; set; }

    public decimal? Fat { get; set; }

    public decimal? Fiber { get; set; }

    public decimal? Sodium { get; set; }
}
