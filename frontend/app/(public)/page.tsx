import { AlertTriangle } from "lucide-react";
import { apiFetch, apiFetchPaged } from "@/lib/api-client";
import { RecipeGrid } from "@/components/recipe/RecipeGrid";
import { CategoryGrid } from "@/components/category/CategoryGrid";
import { HeroSection } from "@/components/home/HeroSection";
import { StatsBar } from "@/components/home/StatsBar";
import { FeatureCards } from "@/components/home/FeatureCards";
import { CtaBanner } from "@/components/home/CtaBanner";
import { SectionHeading } from "@/components/ui/SectionHeading";
import { Reveal } from "@/components/ui/Reveal";
import type { Category } from "@/types/category";
import type { RecipeSummary } from "@/types/recipe";

// Mục 9: "/" — ISR revalidate=3600 (1 giờ). Next.js tự cache HTML và chỉ render lại
// sau khi hết thời gian này, giúp trang chủ tải cực nhanh mà vẫn không quá cũ.
export const revalidate = 3600;

/**
 * Gọi API lấy dữ liệu trang chủ, KHÔNG để lỗi văng ra ngoài Server Component.
 * Lý do: nếu backend .NET chưa chạy (vd. lúc intern chỉ đang làm frontend), `fetch()` sẽ
 * ném lỗi "fetch failed" (ECONNREFUSED) — nếu không catch ở đây, cả trang chủ sẽ crash với
 * màn hình đỏ của Next.js thay vì hiển thị UI bình thường kèm thông báo nhẹ nhàng.
 */
async function getHomeData() {
  try {
    const [recipesPage, categories] = await Promise.all([
      apiFetchPaged<RecipeSummary>("/recipes", {
        params: { page: 1, pageSize: 8, sort: "-createdAt" },
        next: { revalidate },
      }),
      apiFetch<Category[]>("/categories", { next: { revalidate } }),
    ]);
    return {
      recipes: recipesPage.items,
      totalRecipes: recipesPage.meta.total,
      categories,
      backendError: false,
    };
  } catch {
    return {
      recipes: [] as RecipeSummary[],
      totalRecipes: 0,
      categories: [] as Category[],
      backendError: true,
    };
  }
}

/** Trang chủ: hero + thống kê + công thức nổi bật + danh mục. */
export default async function HomePage() {
  const { recipes, totalRecipes, categories, backendError } = await getHomeData();

  return (
    <div className="flex flex-col gap-16 pb-20 sm:gap-20">
      <HeroSection />

      <StatsBar recipeCount={totalRecipes} categoryCount={categories.length} />

      {backendError && (
        <div className="container-page">
          <p className="flex items-start gap-3 rounded-card border border-amber-300 bg-amber-50 px-4 py-3 text-sm text-amber-900">
            <AlertTriangle className="mt-0.5 h-5 w-5 shrink-0 text-amber-600" />
            <span>
              Chưa kết nối được tới backend ({process.env.NEXT_PUBLIC_API_BASE_URL}) — kiểm tra API .NET đã
              chạy chưa. Giao diện vẫn hiển thị bình thường, dữ liệu công thức/danh mục sẽ tự lên khi backend
              sẵn sàng.
            </span>
          </p>
        </div>
      )}

      <FeatureCards />

      <section className="container-page">
        <Reveal>
          <SectionHeading
            eyebrow="Mới nhất"
            title="Công thức vừa được chia sẻ"
            description="Những món mới nhất từ cộng đồng — cập nhật mỗi giờ."
            moreHref="/recipes"
          />
        </Reveal>
        <RecipeGrid recipes={recipes} />
      </section>

      <section className="container-page">
        <Reveal>
          <SectionHeading
            eyebrow="Khám phá"
            title="Duyệt theo danh mục"
            description="Từ món khai vị đến tráng miệng — chọn nhóm món bạn đang thèm."
            moreHref="/categories"
          />
        </Reveal>
        <CategoryGrid categories={categories} />
      </section>

      <CtaBanner />
    </div>
  );
}
