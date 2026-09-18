import Link from "next/link";
import { AuthShell } from "@/components/layout/AuthShell";
import { RegisterForm } from "@/components/forms/RegisterForm";

export const metadata = { title: "Đăng ký" };

// Mục 9: "/auth/register" — CSR.
export default function RegisterPage() {
  return (
    <AuthShell
      title="Tạo tài khoản"
      subtitle="Vài giây là bạn có thể bắt đầu đăng công thức"
      footer={
        <>
          Đã có tài khoản?{" "}
          <Link href="/login" className="font-semibold text-brand-700 hover:underline">
            Đăng nhập
          </Link>
        </>
      }
    >
      <RegisterForm />
    </AuthShell>
  );
}
