"use client";

import { useRouter, useSearchParams } from "next/navigation";
import { Select } from "@/components/ui/Input";
import { useCategories } from "@/hooks/useCategories";
import { DIFFICULTY_LABEL } from "@/lib/utils";

const SORT_OPTIONS = [
  { value: "-createdAt", label: "Mới nhất" },
  { value: "title", label: "Tên A-Z" },
  { value: "cookTime", label: "Thời gian nấu tăng dần" },
];

/**
 * Thanh filter cho trang /recipes (FR-SRCH-002/003/004): categoryId, difficulty, sort.
 * Đọc/ghi trực tiếp vào query string của URL để filter có thể chia sẻ link, back/forward được.
 */
export function RecipeFilters() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const { data: categories } = useCategories();

  function updateParam(key: string, value: string) {
    const params = new URLSearchParams(searchParams.toString());
    if (value) params.set(key, value);
    else params.delete(key);
    params.delete("page"); // đổi filter thì quay về trang 1
    router.push(`/recipes?${params.toString()}`);
  }

  return (
    <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
      <Select
        aria-label="Danh mục"
        placeholder="Tất cả danh mục"
        value={searchParams.get("categoryId") ?? ""}
        onChange={(e) => updateParam("categoryId", e.target.value)}
        options={(categories ?? []).map((c) => ({ value: c.id, label: c.name }))}
      />
      <Select
        aria-label="Độ khó"
        placeholder="Mọi độ khó"
        value={searchParams.get("difficulty") ?? ""}
        onChange={(e) => updateParam("difficulty", e.target.value)}
        options={Object.entries(DIFFICULTY_LABEL).map(([value, label]) => ({ value, label }))}
      />
      <Select
        aria-label="Sắp xếp"
        value={searchParams.get("sort") ?? "-createdAt"}
        onChange={(e) => updateParam("sort", e.target.value)}
        options={SORT_OPTIONS}
      />
    </div>
  );
}
