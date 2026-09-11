import { notFound } from "next/navigation";
import { apiFetch, apiFetchPaged, ApiError } from "@/lib/api-client";
import { RecipeGrid } from "@/components/recipe/RecipeGrid";
import { Pagination } from "@/components/ui/Pagination";
import type { Category } from "@/types/category";
import type { RecipeSummary } from "@/types/recipe";

// Mục 9: "/categories/[slug]" — ISR revalidate=600 (10 phút).
export const revalidate = 600;

interface CategoryPageProps {
  params: Promise<{ slug: string }>;
  searchParams: Promise<{ page?: string }>;
}

export default async function CategoryDetailPage({ params, searchParams }: CategoryPageProps) {
  const { slug } = await params;
  const { page: pageParam } = await searchParams;
  const page = Number(pageParam ?? 1);

  let category: Category;
  try {
    // FR-CAT-002: GET /categories/{slug}?page=&pageSize= — trả kèm recipe Published thuộc category.
    category = await apiFetch<Category>(`/categories/${slug}`, { next: { revalidate } });
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) notFound();
    throw error;
  }

  const { items: recipes, meta } = await apiFetchPaged<RecipeSummary>("/recipes", {
    params: { categoryId: category.id, page, pageSize: 12 },
    next: { revalidate },
  });

  return (
    <div className="container-page flex flex-col gap-6 py-10">
      <div>
        <h1 className="text-2xl font-bold text-neutral-900">{category.name}</h1>
        {category.description && <p className="mt-2 text-neutral-600">{category.description}</p>}
        <p className="mt-1 text-sm text-neutral-500">{category.recipeCount} công thức</p>
      </div>
      <RecipeGrid recipes={recipes} />
      <Pagination meta={meta} buildHref={(p) => `/categories/${slug}?page=${p}`} />
    </div>
  );
}
