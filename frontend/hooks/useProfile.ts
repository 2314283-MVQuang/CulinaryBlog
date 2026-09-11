"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { apiFetch } from "@/lib/api-client";
import { useCurrentUser } from "./useAuth";
import type { UpdateProfileInput, UserProfile } from "@/types/user";

export const profileKeys = {
  me: ["profile", "me"] as const,
};

/** FR-AUTH-006: GET /auth/me — hồ sơ đầy đủ của user đang đăng nhập (bio/avatar không có trong session JWT). */
export function useMyProfile() {
  const { accessToken, isAuthenticated } = useCurrentUser();

  return useQuery({
    queryKey: profileKeys.me,
    queryFn: () => apiFetch<UserProfile>("/auth/me", { token: accessToken }),
    enabled: isAuthenticated,
  });
}

/**
 * FR-AUTH-007: PATCH /auth/me — chỉ đổi displayName/bio/avatarUrl.
 * Ghi chú cho intern: cập nhật xong chỉ đổi cache của useMyProfile(), CHƯA làm mới session JWT
 * (tên/avatar trong Header vẫn lấy từ session cũ cho tới lần đăng nhập sau). Muốn đồng bộ ngay,
 * gọi thêm `update()` từ `useSession()` của next-auth/react tại đây.
 */
export function useUpdateProfile() {
  const queryClient = useQueryClient();
  const { accessToken } = useCurrentUser();

  return useMutation({
    mutationFn: (input: UpdateProfileInput) =>
      apiFetch<UserProfile>("/auth/me", { method: "PATCH", body: input, token: accessToken }),
    onSuccess: (data) => {
      queryClient.setQueryData(profileKeys.me, data);
    },
  });
}
