import { RecipeCard } from "./RecipeCard";
import { Reveal } from "@/components/ui/Reveal";
import type { RecipeSummary } from "@/types/recipe";

/**
 * Lưới hiển thị nhiều RecipeCard, responsive 1/2/3/4 cột — dùng lại ở mọi trang danh sách recipe.
 *
 * Mỗi card được bọc trong <Reveal> với độ trễ tăng dần, nên khi cuộn tới, các card hiện ra
 * lần lượt như quân domino thay vì bật lên cùng lúc (hiệu ứng "stagger").
 * Độ trễ giới hạn ở 400ms để card cuối cùng không phải chờ quá lâu.
 */
export function RecipeGrid({ recipes }: { recipes: RecipeSummary[] }) {
  if (recipes.length === 0) {
    return (
      <div className="rounded-card-lg border border-dashed border-brand-200 bg-white/60 py-16 text-center">
        <span className="text-5xl" aria-hidden>
          🍳
        </span>
        <p className="mt-3 font-display text-lg font-semibold text-neutral-700">Chưa có công thức nào</p>
        <p className="mt-1 text-sm text-neutral-500">
          Khi backend có dữ liệu, các công thức sẽ hiện ở đây.
        </p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
      {recipes.map((recipe, index) => (
        <Reveal key={recipe.id} delay={Math.min(index * 80, 400)} className="h-full">
          <RecipeCard recipe={recipe} />
        </Reveal>
      ))}
    </div>
  );
}
