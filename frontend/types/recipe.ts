import type { Category } from "./category";
import type { UserProfile } from "./user";

/** Khớp enum RecipeDifficulty (mục 7.2): 1=Easy 2=Medium 3=Hard 4=Expert. */
export type RecipeDifficulty = "Easy" | "Medium" | "Hard" | "Expert";

/** Khớp enum RecipeStatus (mục 7.2): 0=Draft 1=Published 2=Archived. */
export type RecipeStatus = "Draft" | "Published" | "Archived";

/** RecipeNutrition — Owned Entity, tất cả field đều optional (mục 7.2). */
export interface RecipeNutrition {
  calories: number | null;
  protein: number | null;
  carbohydrates: number | null;
  fat: number | null;
  fiber: number | null;
  sodium: number | null;
}

/** Khớp bảng "RecipeSteps" (mục 7.3). */
export interface RecipeStep {
  id: string;
  stepNumber: number;
  title: string;
  description: string;
  timerMinutes: number | null;
  imageUrl: string | null;
}

/** Khớp bảng "RecipeIngredients" (mục 7.4). */
export interface RecipeIngredient {
  id: string;
  name: string;
  quantity: number | null;
  unit: string | null;
  notes: string | null;
  orderIndex: number;
}

/** Khớp bảng "RecipeImages" (mục 7.5). */
export interface RecipeImage {
  id: string;
  originalUrl: string;
  mediumUrl: string | null;
  thumbnailUrl: string | null;
  altText: string | null;
  isPrimary: boolean;
  orderIndex: number;
}

/**
 * Dạng rút gọn dùng cho danh sách (GET /recipes) — chỉ có 1 ảnh đại diện thay vì mảng đầy đủ,
 * để payload nhẹ hơn khi hiển thị dạng lưới (RecipeCard).
 */
export interface RecipeSummary {
  id: string;
  slug: string;
  title: string;
  description: string;
  prepTime: number;
  cookTime: number;
  servings: number;
  difficulty: RecipeDifficulty;
  status: RecipeStatus;
  category: Pick<Category, "id" | "name" | "slug">;
  author: Pick<UserProfile, "id" | "displayName" | "avatarUrl">;
  primaryImageUrl: string | null;
  publishedAt: string | null;
  createdAt: string;
}

/** Chi tiết đầy đủ 1 recipe, dùng cho GET /recipes/{slug} (mục 8.3, FR-RCP-002). */
export interface RecipeDetail extends RecipeSummary {
  instructions: string;
  nutrition: RecipeNutrition;
  steps: RecipeStep[];
  ingredients: RecipeIngredient[];
  images: RecipeImage[];
  /** RowVersion dạng base64 — bắt buộc gửi lại trong header If-Match khi PUT (FR-RCP-004, ETag pattern). */
  rowVersion: string;
}

/** Query params cho GET /recipes (mục 8.3, FR-SRCH-002/003/004). */
export interface RecipeListParams {
  page?: number;
  pageSize?: number;
  categoryId?: string;
  difficulty?: RecipeDifficulty;
  maxCookTime?: number;
  minServings?: number;
  sort?: string; // vd. "-createdAt", "title", "cookTime"
  /**
   * Lọc theo tác giả — dùng ở trang "/dashboard/recipes" (công thức của tôi).
   * Intern lưu ý: kiểm tra lại với Swagger backend xem param này tên đúng là "authorId" không
   * trước khi dùng ở production — nếu backend chưa hỗ trợ, useRecipes() vẫn lọc lại phía client
   * bằng recipe.author.id nên trang vẫn hiển thị đúng dữ liệu (chỉ là tải dư item không cần thiết).
   */
  authorId?: string;
}

/** Query params cho GET /recipes/search (FR-SRCH-001). */
export interface RecipeSearchParams extends RecipeListParams {
  q: string;
}

/** Body tạo recipe mới (FR-RCP-003) — trạng thái ban đầu luôn là Draft, server tự set. */
export interface CreateRecipeInput {
  title: string;
  description: string;
  categoryId: string;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  servings: number;
  difficulty: RecipeDifficulty;
  instructions?: string;
  nutrition?: Partial<RecipeNutrition>;
  steps?: Array<Pick<RecipeStep, "title" | "description" | "timerMinutes">>;
  ingredients?: Array<Pick<RecipeIngredient, "name" | "quantity" | "unit" | "notes">>;
}

/** Body cập nhật recipe (FR-RCP-004) — mọi field optional vì có thể chỉ sửa 1 phần. */
export type UpdateRecipeInput = Partial<CreateRecipeInput>;
