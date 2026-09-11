"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { Button } from "@/components/ui/Button";
import { Input, Textarea } from "@/components/ui/Input";
import { useUpdateProfile } from "@/hooks/useProfile";
import { updateProfileSchema, type UpdateProfileFormValues } from "@/lib/validations/auth.schema";
import type { UserProfile } from "@/types/user";

/** Form xem/sửa hồ sơ cá nhân (FR-AUTH-007, mục 9: "/profile"). */
export function ProfileForm({ user }: { user: UserProfile }) {
  const updateProfile = useUpdateProfile();
  const [savedMessage, setSavedMessage] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<UpdateProfileFormValues>({
    resolver: zodResolver(updateProfileSchema),
    defaultValues: {
      displayName: user.displayName,
      bio: user.bio ?? "",
      avatarUrl: user.avatarUrl ?? "",
    },
  });

  async function onSubmit(values: UpdateProfileFormValues) {
    setSavedMessage(false);
    await updateProfile.mutateAsync({
      displayName: values.displayName,
      bio: values.bio || undefined,
      avatarUrl: values.avatarUrl || undefined,
    });
    setSavedMessage(true);
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4">
      <div className="flex items-center gap-2 text-sm text-neutral-500">
        <span className="font-medium text-neutral-700">Email:</span>
        {user.email}
        <span className="ml-auto rounded-full bg-neutral-100 px-2 py-0.5 text-xs">{user.roles.join(", ")}</span>
      </div>

      <Input label="Tên hiển thị" error={errors.displayName?.message} {...register("displayName")} />
      <Textarea label="Giới thiệu bản thân" error={errors.bio?.message} {...register("bio")} />
      <Input label="URL ảnh đại diện" placeholder="https://..." error={errors.avatarUrl?.message} {...register("avatarUrl")} />

      {savedMessage && (
        <p className="rounded-lg bg-green-50 px-3 py-2 text-sm text-green-700">Đã lưu thay đổi.</p>
      )}
      {updateProfile.isError && (
        <p className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">Lưu thất bại, vui lòng thử lại.</p>
      )}

      <div className="flex justify-end">
        <Button type="submit" isLoading={updateProfile.isPending}>
          Lưu thay đổi
        </Button>
      </div>
    </form>
  );
}
