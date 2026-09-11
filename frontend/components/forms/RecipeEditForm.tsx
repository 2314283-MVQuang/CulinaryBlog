"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { Button } from "@/components/ui/Button";
import { Input, Select, Textarea } from "@/components/ui/Input";
import { IngredientList } from "@/components/recipe/IngredientList";
import { StepList } from "@/components/recipe/StepList";
import { StatusBadge } from "@/components/recipe/StatusBadge";
import { useCategories } from "@/hooks/useCategories";
import {
  useArchiveRecipe,
  useDeleteRecipe,
  usePublishRecipe,
  useUnpublishRecipe,
  useUpdateRecipe,
} from "@/hooks/useRecipes";
import { recipeBasicInfoSchema, type RecipeBasicInfoValues } from "@/lib/validations/recipe.schema";
import { DIFFICULTY_LABEL } from "@/lib/utils";
import { ApiError } from "@/lib/api-client";
import { ApiErrorCode } from "@/types/common";
import type { RecipeDetail } from "@/types/recipe";

/**
 * Form chỉnh sửa recipe (FR-RCP-004) + các hành động publish/unpublish/archive/delete.
 * Concurrency: mỗi lần Update đều gửi kèm `recipe.rowVersion` qua header If-Match —
 * nếu người khác vừa sửa trước đó, backend trả 409 và ta báo lỗi thay vì ghi đè âm thầm.
 *
 * Ghi chú cho intern: sửa/xoá TỪNG ingredient hoặc step riêng lẻ (FR-RCP-009/010) chưa được
 * nối dây ở bản khung này — danh sách bên dưới hiện tại chỉ hiển thị read-only. Muốn thêm,
 * làm theo đúng pattern của hooks/useRecipes.ts (thêm hook mới gọi PUT/DELETE endpoint tương ứng).
 */
export function RecipeEditForm({ recipe }: { recipe: RecipeDetail }) {
  const router = useRouter();
  const { data: categories } = useCategories();

  const updateRecipe = useUpdateRecipe();
  const publishRecipe = usePublishRecipe();
  const unpublishRecipe = useUnpublishRecipe();
  const archiveRecipe = useArchiveRecipe();
  const deleteRecipe = useDeleteRecipe();

  const {
    register,
    handleSubmit,
    setError,
    formState: { errors, isSubmitting },
  } = useForm<RecipeBasicInfoValues>({
    resolver: zodResolver(recipeBasicInfoSchema),
    defaultValues: {
      title: recipe.title,
      description: recipe.description,
      categoryId: recipe.category.id,
      prepTimeMinutes: recipe.prepTime,
      cookTimeMinutes: recipe.cookTime,
      servings: recipe.servings,
      difficulty: recipe.difficulty,
    },
  });

  async function onSubmit(values: RecipeBasicInfoValues) {
    try {
      await updateRecipe.mutateAsync({ id: recipe.id, input: values, rowVersion: recipe.rowVersion });
    } catch (error) {
      if (error instanceof ApiError && error.type === ApiErrorCode.RecipeConcurrencyConflict) {
        setError("root", { message: "Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang." });
      } else {
        setError("root", { message: "Lưu thất bại, vui lòng thử lại." });
      }
    }
  }

  async function handleDelete() {
    if (!confirm("Xoá vĩnh viễn công thức này? Hành động không thể hoàn tác.")) return;
    await deleteRecipe.mutateAsync(recipe.id);
    router.push("/dashboard/recipes");
  }

  return (
    <div className="flex flex-col gap-8">
      <div className="flex flex-wrap items-center justify-between gap-3 rounded-card border border-neutral-200 bg-neutral-50 p-4">
        <div className="flex items-center gap-2">
          <span className="text-sm text-neutral-600">Trạng thái:</span>
          <StatusBadge status={recipe.status} />
        </div>
        <div className="flex flex-wrap gap-2">
          {recipe.status !== "Published" && (
            <Button
              size="sm"
              variant="outline"
              isLoading={publishRecipe.isPending}
              onClick={() => publishRecipe.mutate(recipe.id)}
            >
              Publish
            </Button>
          )}
          {recipe.status === "Published" && (
            <Button
              size="sm"
              variant="outline"
              isLoading={unpublishRecipe.isPending}
              onClick={() => unpublishRecipe.mutate(recipe.id)}
            >
              Unpublish
            </Button>
          )}
          {recipe.status !== "Archived" && (
            <Button
              size="sm"
              variant="outline"
              isLoading={archiveRecipe.isPending}
              onClick={() => archiveRecipe.mutate(recipe.id)}
            >
              Lưu trữ
            </Button>
          )}
          <Button size="sm" variant="danger" isLoading={deleteRecipe.isPending} onClick={handleDelete}>
            Xoá
          </Button>
        </div>
      </div>
      {publishRecipe.isError && (
        <p className="text-sm text-red-600">
          Không thể publish — công thức cần có ít nhất 1 bước thực hiện (FR-RCP-005).
        </p>
      )}

      <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4">
        <h2 className="font-semibold text-neutral-900">Thông tin cơ bản</h2>
        {errors.root && <p className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">{errors.root.message}</p>}

        <Input label="Tên công thức" error={errors.title?.message} {...register("title")} />
        <Textarea label="Mô tả ngắn" error={errors.description?.message} {...register("description")} />
        <Select
          label="Danh mục"
          error={errors.categoryId?.message}
          options={(categories ?? []).map((c) => ({ value: c.id, label: c.name }))}
          {...register("categoryId")}
        />
        <div className="grid grid-cols-3 gap-4">
          <Input label="Chuẩn bị (phút)" type="number" error={errors.prepTimeMinutes?.message} {...register("prepTimeMinutes")} />
          <Input label="Thời gian nấu (phút)" type="number" error={errors.cookTimeMinutes?.message} {...register("cookTimeMinutes")} />
          <Input label="Khẩu phần" type="number" error={errors.servings?.message} {...register("servings")} />
        </div>
        <Select
          label="Độ khó"
          options={Object.entries(DIFFICULTY_LABEL).map(([value, label]) => ({ value, label }))}
          error={errors.difficulty?.message}
          {...register("difficulty")}
        />

        <div className="flex justify-end">
          <Button type="submit" isLoading={isSubmitting || updateRecipe.isPending}>
            Lưu thay đổi
          </Button>
        </div>
      </form>

      <div>
        <h2 className="mb-3 font-semibold text-neutral-900">Nguyên liệu ({recipe.ingredients.length})</h2>
        <IngredientList ingredients={recipe.ingredients} />
      </div>

      <div>
        <h2 className="mb-3 font-semibold text-neutral-900">Các bước thực hiện ({recipe.steps.length})</h2>
        <StepList steps={recipe.steps} />
      </div>
    </div>
  );
}
