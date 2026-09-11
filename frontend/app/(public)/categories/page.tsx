import { AlertTriangle } from "lucide-react";
import { apiFetch } from "@/lib/api-client";
import { CategoryGrid } from "@/components/category/CategoryGrid";
import { PageHero } from "@/components/layout/PageHero";
import type { Category } from "@/types/category";

// Mục 9: "/categories" — ISR revalidate=3600.
export const revalidate = 3600;

export default async function CategoriesPage() {
  // Bọc try-catch để backend chưa chạy thì trang vẫn render được (xem chú thích ở trang chủ).
  let categories: Category[] = [];
  let backendError = false;

  try {
    categories = await apiFetch<Category[]>("/categories", { next: { revalidate } });
  } catch {
    backendError = true;
  }

  return (
    <div className="flex flex-col">
      <PageHero
        title="Danh mục công thức"
        description="Từ món khai vị đến tráng miệng — chọn nhóm món để xem toàn bộ công thức thuộc nhóm đó."
        emojis={["🥗", "🍰", "🍲"]}
      >
        <span className="inline-flex items-center gap-2 rounded-full border border-white/30 bg-white/15 px-4 py-1.5 text-sm font-medium text-white backdrop-blur-md">
          {categories.length} danh mục
        </span>
      </PageHero>

      <div className="container-page flex flex-col gap-6 pb-20 pt-8">
        {backendError && (
          <p className="flex items-start gap-3 rounded-card border border-amber-300 bg-amber-50 px-4 py-3 text-sm text-amber-900">
            <AlertTriangle className="mt-0.5 h-5 w-5 shrink-0 text-amber-600" />
            <span>Chưa kết nối được tới backend — danh mục sẽ hiện khi API .NET chạy.</span>
          </p>
        )}

        <CategoryGrid categories={categories} />
      </div>
    </div>
  );
}
