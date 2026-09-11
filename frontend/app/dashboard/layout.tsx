import Link from "next/link";
import { LayoutGrid, BookOpen, FolderTree } from "lucide-react";

const NAV = [
  { href: "/dashboard", label: "Tổng quan", icon: LayoutGrid },
  { href: "/dashboard/recipes", label: "Công thức của tôi", icon: BookOpen },
  { href: "/dashboard/categories", label: "Danh mục", icon: FolderTree, adminOnly: true },
];

/**
 * Layout cho toàn bộ /dashboard/** — sidebar điều hướng. Việc chặn truy cập khi chưa đăng nhập
 * đã xử lý ở middleware.ts (route matcher "/dashboard/:path*"), layout này không cần check lại.
 * "/dashboard/categories" bị middleware chặn riêng nếu không phải Admin, nên sidebar vẫn hiện
 * link cho mọi người — Author bấm vào sẽ bị điều hướng về /dashboard (không hiện lỗi khó chịu).
 */
export default function DashboardLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="container-page grid grid-cols-1 gap-8 py-10 md:grid-cols-[220px_1fr]">
      <aside className="flex flex-col gap-1">
        {NAV.map((item) => (
          <Link
            key={item.href}
            href={item.href}
            className="flex items-center gap-2 rounded-lg px-3 py-2 text-sm font-medium text-neutral-600 hover:bg-neutral-100 hover:text-neutral-900"
          >
            <item.icon className="h-4 w-4" />
            {item.label}
          </Link>
        ))}
      </aside>
      <div>{children}</div>
    </div>
  );
}
