import { AlertTriangle, SearchX } from "lucide-react";
import { apiFetchPaged } from "@/lib/api-client";
import { RecipeGrid } from "@/components/recipe/RecipeGrid";
import { PageHero } from "@/components/layout/PageHero";
import { Pagination } from "@/components/ui/Pagination";
import type { PagedResult } from "@/types/common";
import type { RecipeSummary } from "@/types/recipe";

// Mục 9: "/search" — SSR, kết quả phụ thuộc query "q" nên không cache.
export const dynamic = "force-dynamic";

interface SearchPageProps {
  searchParams: Promise<{ q?: string; page?: string }>;
}

export default async function SearchPage({ searchParams }: SearchPageProps) {
  const { q, page: pageParam } = await searchParams;
  const query = (q ?? "").trim();
  const page = Number(pageParam ?? 1);

  // FR-SRCH-001: backend trả 422 nếu q < 2 ký tự — chặn sớm ở FE, không gọi API vô ích.
  const hasValidQuery = query.length >= 2;

  let result: PagedResult<RecipeSummary> | null = null;
  let backendError = false;

  if (hasValidQuery) {
    try {
      result = await apiFetchPaged<RecipeSummary>("/recipes/search", {
        params: { q: query, page, pageSize: 12 },
      });
    } catch {
      backendError = true;
    }
  }

  return (
    <div className="flex flex-col">
      <PageHero
        title={hasValidQuery ? `Kết quả cho “${query}”` : "Tìm kiếm công thức"}
        description={
          hasValidQuery
            ? "Không thấy món bạn cần? Thử từ khoá ngắn hơn, hoặc tìm theo tên nguyên liệu chính."
            : "Nhập ít nhất 2 ký tự vào ô tìm kiếm ở thanh trên cùng để bắt đầu."
        }
        emojis={["🔎", "🍲"]}
      >
        {result && (
          <span className="inline-flex items-center gap-2 rounded-full border border-white/30 bg-white/15 px-4 py-1.5 text-sm font-medium text-white backdrop-blur-md">
            {result.meta.total} kết quả
          </span>
        )}
      </PageHero>

      <div className="container-page flex flex-col gap-6 pb-20 pt-8">
        {backendError && (
          <p className="flex items-start gap-3 rounded-card border border-amber-300 bg-amber-50 px-4 py-3 text-sm text-amber-900">
            <AlertTriangle className="mt-0.5 h-5 w-5 shrink-0 text-amber-600" />
            <span>Chưa kết nối được tới backend — kết quả sẽ hiện khi API .NET chạy.</span>
          </p>
        )}

        {!hasValidQuery && (
          <div className="rounded-card-lg border border-dashed border-brand-200 bg-white/60 py-16 text-center">
            <SearchX className="mx-auto h-10 w-10 text-brand-300" />
            <p className="mt-3 font-display text-lg font-semibold text-neutral-700">Chưa có từ khoá</p>
            <p className="mt-1 text-sm text-neutral-500">
              Gõ tên món, nguyên liệu hoặc kiểu chế biến vào ô tìm kiếm phía trên.
            </p>
          </div>
        )}

        {result && (
          <>
            <RecipeGrid recipes={result.items} />
            <Pagination
              meta={result.meta}
              buildHref={(p) => `/search?q=${encodeURIComponent(query)}&page=${p}`}
            />
          </>
        )}
      </div>
    </div>
  );
}
