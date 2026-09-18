import Link from "next/link";
import { Suspense } from "react";
import { AuthShell } from "@/components/layout/AuthShell";
import { LoginForm } from "@/components/forms/LoginForm";

export const metadata = { title: "Đăng nhập" };

// Mục 9: "/auth/login" — CSR (form tương tác), redirect nếu đã đăng nhập (xử lý ở middleware.ts
// khi user cố vào /dashboard mà chưa login sẽ được đưa về đây, không phải chiều ngược lại ở đây).
export default function LoginPage() {
  return (
    <AuthShell
      title="Chào mừng trở lại"
      subtitle="Đăng nhập để quản lý công thức của bạn"
      footer={
        <>
          Chưa có tài khoản?{" "}
          <Link href="/register" className="font-semibold text-brand-700 hover:underline">
            Đăng ký ngay
          </Link>
        </>
      }
    >
      {/* useSearchParams() bên trong LoginForm cần bọc Suspense theo yêu cầu của Next.js App Router. */}
      <Suspense fallback={<div className="skeleton h-56 w-full" />}>
        <LoginForm />
      </Suspense>
    </AuthShell>
  );
}
