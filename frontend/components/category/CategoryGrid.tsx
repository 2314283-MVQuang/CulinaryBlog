import { CategoryCard } from "./CategoryCard";
import { Reveal } from "@/components/ui/Reveal";
import type { Category } from "@/types/category";

/** Lưới danh mục — các card hiện lần lượt khi cuộn tới (xem chú thích ở RecipeGrid). */
export function CategoryGrid({ categories }: { categories: Category[] }) {
  if (categories.length === 0) {
    return (
      <div className="rounded-card-lg border border-dashed border-brand-200 bg-white/60 py-16 text-center">
        <span className="text-5xl" aria-hidden>
          🗂️
        </span>
        <p className="mt-3 font-display text-lg font-semibold text-neutral-700">Chưa có danh mục nào</p>
        <p className="mt-1 text-sm text-neutral-500">Admin có thể thêm danh mục trong trang quản lý.</p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4 sm:gap-6">
      {categories.map((category, index) => (
        <Reveal key={category.id} delay={Math.min(index * 70, 350)} className="h-full">
          <CategoryCard category={category} />
        </Reveal>
      ))}
    </div>
  );
}
