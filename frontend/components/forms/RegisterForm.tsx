"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { signIn } from "next-auth/react";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { apiFetch, ApiError } from "@/lib/api-client";
import { registerSchema, type RegisterFormValues } from "@/lib/validations/auth.schema";
import { ApiErrorCode } from "@/types/common";

/** Form đăng ký tài khoản mới (FR-AUTH-001) — sau khi đăng ký thành công, tự đăng nhập luôn. */
export function RegisterForm() {
  const router = useRouter();
  const [formError, setFormError] = useState<string | null>(null);

  const {
    register: registerField,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<RegisterFormValues>({ resolver: zodResolver(registerSchema) });

  async function onSubmit(values: RegisterFormValues) {
    setFormError(null);
    try {
      // Đăng ký không nằm trong provider của next-auth (không phải "login"), nên gọi thẳng API.
      await apiFetch("/auth/register", { method: "POST", body: values });

      // Đăng ký xong thì tự đăng nhập bằng chính email/password vừa nhập, đỡ bắt user gõ lại.
      await signIn("credentials", { email: values.email, password: values.password, redirect: false });
      router.push("/dashboard");
      router.refresh();
    } catch (error) {
      if (error instanceof ApiError && error.type === ApiErrorCode.AuthEmailExists) {
        setFormError("Email này đã được đăng ký. Vui lòng đăng nhập hoặc dùng email khác.");
      } else {
        setFormError("Đăng ký thất bại, vui lòng thử lại.");
      }
    }
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4">
      {formError && (
        <p role="alert" className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700">
          {formError}
        </p>
      )}

      <Input label="Họ và tên" error={errors.fullName?.message} {...registerField("fullName")} />
      <Input label="Email" type="email" autoComplete="email" error={errors.email?.message} {...registerField("email")} />
      <Input label="Tên đăng nhập" error={errors.userName?.message} {...registerField("userName")} />
      <Input
        label="Mật khẩu"
        type="password"
        autoComplete="new-password"
        hint="Tối thiểu 8 ký tự, có chữ hoa, số và ký tự đặc biệt."
        error={errors.password?.message}
        {...registerField("password")}
      />

      <Button type="submit" isLoading={isSubmitting} className="mt-2">
        Đăng ký
      </Button>
      {/* Link "Đã có tài khoản?" nằm ở AuthShell (dưới thẻ form) nên không lặp lại ở đây. */}
    </form>
  );
}
