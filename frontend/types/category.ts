/** Khớp bảng "Categories" (mục 7.6) + recipeCount trả kèm ở GET /categories (mục 8.2). */
export interface Category {
  id: string;
  name: string;
  slug: string;
  description: string | null;
  imageUrl: string | null;
  recipeCount: number;
}

/** Body tạo/sửa category (FR-CAT-003, FR-CAT-004) — Admin only. */
export interface CategoryInput {
  name: string;
  description?: string;
  imageUrl?: string;
}
