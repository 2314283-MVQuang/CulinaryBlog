"use client";

import { useState } from "react";
import { Pencil, Plus, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/Button";
import { ErrorBlock, LoadingBlock } from "@/components/ui/Spinner";
import { CategoryForm } from "@/components/forms/CategoryForm";
import {
  useCategories,
  useCreateCategory,
  useDeleteCategory,
  useUpdateCategory,
} from "@/hooks/useCategories";
import { ApiError } from "@/lib/api-client";
import type { CategoryFormValues } from "@/lib/validations/recipe.schema";
import type { Category } from "@/types/category";

/**
 * Mục 9: "/dashboard/categories" — CSR, chỉ Admin (middleware.ts đã chặn Author truy cập route
 * này ở tầng route — component không cần tự kiểm tra role lại).
 */
export default function DashboardCategoriesPage() {
  const { data: categories, isLoading, isError, refetch } = useCategories();
  const createCategory = useCreateCategory();
  const updateCategory = useUpdateCategory();
  const deleteCategory = useDeleteCategory();

  const [mode, setMode] = useState<"none" | "create" | Category>("none");
  const [deleteErrorId, setDeleteErrorId] = useState<string | null>(null);

  async function handleCreate(values: CategoryFormValues) {
    await createCategory.mutateAsync(values);
    setMode("none");
  }

  async function handleUpdate(id: string, values: CategoryFormValues) {
    await updateCategory.mutateAsync({ id, input: values });
    setMode("none");
  }

  async function handleDelete(category: Category) {
    if (!confirm(`Xoá danh mục "${category.name}"?`)) return;
    setDeleteErrorId(null);
    try {
      await deleteCategory.mutateAsync(category.id);
    } catch (error) {
      // FR-CAT-005: backend trả 409 nếu category còn recipe đang tham chiếu.
      if (error instanceof ApiError && error.status === 409) {
        setDeleteErrorId(category.id);
      }
    }
  }

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-neutral-900">Quản lý danh mục</h1>
        {mode === "none" && (
          <Button onClick={() => setMode("create")}>
            <Plus className="mr-2 h-4 w-4" />
            Thêm danh mục
          </Button>
        )}
      </div>

      {mode === "create" && (
        <div className="rounded-card border border-neutral-200 p-6">
          <h2 className="mb-4 font-semibold text-neutral-900">Danh mục mới</h2>
          <CategoryForm onSubmit={handleCreate} onCancel={() => setMode("none")} isSubmitting={createCategory.isPending} />
        </div>
      )}

      {mode !== "none" && mode !== "create" && (
        <div className="rounded-card border border-neutral-200 p-6">
          <h2 className="mb-4 font-semibold text-neutral-900">Sửa danh mục</h2>
          <CategoryForm
            initialData={mode}
            onSubmit={(values) => handleUpdate(mode.id, values)}
            onCancel={() => setMode("none")}
            isSubmitting={updateCategory.isPending}
          />
        </div>
      )}

      {isLoading && <LoadingBlock />}
      {isError && <ErrorBlock message="Không tải được danh mục." onRetry={() => refetch()} />}

      {categories && (
        <ul className="divide-y divide-neutral-100 rounded-card border border-neutral-200">
          {categories.length === 0 && <li className="p-4 text-sm text-neutral-400">Chưa có danh mục nào.</li>}
          {categories.map((category) => (
            <li key={category.id} className="flex flex-col gap-2 p-4">
              <div className="flex items-center justify-between gap-4">
                <div>
                  <p className="font-medium text-neutral-900">{category.name}</p>
                  <p className="text-sm text-neutral-500">{category.recipeCount} công thức</p>
                </div>
                <div className="flex gap-2">
                  <Button size="sm" variant="outline" onClick={() => setMode(category)}>
                    <Pencil className="h-4 w-4" />
                  </Button>
                  <Button size="sm" variant="danger" isLoading={deleteCategory.isPending} onClick={() => handleDelete(category)}>
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </div>
              </div>
              {deleteErrorId === category.id && (
                <p className="text-sm text-red-600">
                  Không thể xoá — danh mục này vẫn còn công thức đang sử dụng (FR-CAT-005).
                </p>
              )}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
