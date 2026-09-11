import { AlertTriangle } from "lucide-react";
import { apiFetchPaged } from "@/lib/api-client";
import { RecipeGrid } from "@/components/recipe/RecipeGrid";
import { RecipeFilters } from "@/components/recipe/RecipeFilters";
import { PageHero } from "@/components/layout/PageHero";
import { Pagination } from "@/components/ui/Pagination";
import type { PageMeta } from "@/types/common";
import type { RecipeSummary } from "@/types/recipe";

// Mục 9: "/recipes" — SSR (dynamic), vì kết quả phụ thuộc filter/sort/page trên query string.
export const dynamic = "force-dynamic";

interface RecipesPageProps {
  searchParams: Promise<Record<string, string | undefined>>;
}

const EMPTY_META: PageMeta = { page: 1, pageSize: 12, total: 0, totalPages: 1 };

export default async function RecipesPage({ searchParams }: RecipesPageProps) {
  const params = await searchParams;
  const page = Number(params.page ?? 1);

  // Bọc try-catch để backend chưa chạy thì trang vẫn render được (xem chú thích ở trang chủ).
  let recipes: RecipeSummary[] = [];
  let meta: PageMeta = EMPTY_META;
  let backendError = false;

  try {
    const result = await apiFetchPaged<RecipeSummary>("/recipes", {
      params: {
        page,
        pageSize: 12,
        categoryId: params.categoryId,
        difficulty: params.difficulty,
        maxCookTime: params.maxCookTime,
        sort: params.sort ?? "-createdAt",
      },
    });
    recipes = result.items;
    meta = result.meta;
  } catch {
    backendError = true;
  }

  function buildHref(targetPage: number) {
    const next = new URLSearchParams(params as Record<string, string>);
    next.set("page", String(targetPage));
    return `/recipes?${next.toString()}`;
  }

  return (
    <div className="flex flex-col">
      <PageHero
        title="Tất cả công thức"
        description="Lọc theo danh mục, độ khó hay thời gian nấu để tìm đúng món bạn cần cho hôm nay."
        emojis={["🍜", "🥘", "🍤"]}
      >
        <span className="inline-flex items-center gap-2 rounded-full border border-white/30 bg-white/15 px-4 py-1.5 text-sm font-medium text-white backdrop-blur-md">
          {meta.total} công thức
        </span>
      </PageHero>

      <div className="container-page flex flex-col gap-6 pb-20 pt-8">
        <RecipeFilters />

        {backendError && (
          <p className="flex items-start gap-3 rounded-card border border-amber-300 bg-amber-50 px-4 py-3 text-sm text-amber-900">
            <AlertTriangle className="mt-0.5 h-5 w-5 shrink-0 text-amber-600" />
            <span>Chưa kết nối được tới backend — danh sách sẽ hiện khi API .NET chạy.</span>
          </p>
        )}

        <RecipeGrid recipes={recipes} />
        <Pagination meta={meta} buildHref={buildHref} />
      </div>
    </div>
  );
}
