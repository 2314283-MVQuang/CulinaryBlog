"use client";

import { useParams } from "next/navigation";
import { RecipeEditForm } from "@/components/forms/RecipeEditForm";
import { ErrorBlock, LoadingBlock } from "@/components/ui/Spinner";
import { useRecipe } from "@/hooks/useRecipes";

/**
 * Mục 9: "/dashboard/recipes/[id]/edit" — CSR.
 * Lưu ý cho intern: thư mục đặt tên "[id]" theo đúng route trong tài liệu, NHƯNG giá trị thực
 * tế truyền vào URL là SLUG (RecipeWizardForm/RecipeCard đều link ra dạng
 * `/dashboard/recipes/${recipe.slug}/edit`) — vì backend chỉ có GET /recipes/{slug} (mục 8.3),
 * không có endpoint lấy chi tiết theo id riêng. Dùng useParams() thay vì prop `params` (Promise)
 * vì đây là Client Component.
 */
export default function EditRecipePage() {
  const params = useParams<{ id: string }>();
  const slug = params.id;

  const { data: recipe, isLoading, isError, refetch } = useRecipe(slug);

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold text-neutral-900">Chỉnh sửa công thức</h1>

      {isLoading && <LoadingBlock />}
      {isError && <ErrorBlock message="Không tìm thấy công thức hoặc bạn không có quyền chỉnh sửa." onRetry={() => refetch()} />}
      {recipe && <RecipeEditForm recipe={recipe} />}
    </div>
  );
}
