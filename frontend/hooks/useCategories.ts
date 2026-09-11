"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { apiFetch } from "@/lib/api-client";
import { useCurrentUser } from "./useAuth";
import type { Category, CategoryInput } from "@/types/category";

/** Query key tập trung 1 chỗ — tránh mỗi nơi gõ tay `["categories"]` dễ gõ sai/không đồng bộ. */
export const categoryKeys = {
  all: ["categories"] as const,
  detail: (slug: string) => ["categories", slug] as const,
};

/** FR-CAT-001: GET /categories — public, không cần token. */
export function useCategories() {
  return useQuery({
    queryKey: categoryKeys.all,
    queryFn: () => apiFetch<Category[]>("/categories"),
    staleTime: 60_000, // khớp cache 60 phút phía backend (mục 5.1), FE giữ tươi 1 phút là đủ
  });
}

/** FR-CAT-003: POST /categories — Admin only, cần accessToken. */
export function useCreateCategory() {
  const queryClient = useQueryClient();
  const { accessToken } = useCurrentUser();

  return useMutation({
    mutationFn: (input: CategoryInput) =>
      apiFetch<Category>("/categories", { method: "POST", body: input, token: accessToken }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: categoryKeys.all });
    },
  });
}

/** FR-CAT-004: PUT /categories/{id} — chỉ sửa Name/Description, Slug giữ nguyên. */
export function useUpdateCategory() {
  const queryClient = useQueryClient();
  const { accessToken } = useCurrentUser();

  return useMutation({
    mutationFn: ({ id, input }: { id: string; input: CategoryInput }) =>
      apiFetch<Category>(`/categories/${id}`, { method: "PUT", body: input, token: accessToken }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: categoryKeys.all });
    },
  });
}

/** FR-CAT-005: DELETE /categories/{id} — backend trả 409 nếu category còn recipe. */
export function useDeleteCategory() {
  const queryClient = useQueryClient();
  const { accessToken } = useCurrentUser();

  return useMutation({
    mutationFn: (id: string) =>
      apiFetch<void>(`/categories/${id}`, { method: "DELETE", token: accessToken }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: categoryKeys.all });
    },
  });
}
