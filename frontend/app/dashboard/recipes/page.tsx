"use client";

import Link from "next/link";
import { FilePlus2, PenSquare } from "lucide-react";
import { Button } from "@/components/ui/Button";
import { ErrorBlock, LoadingBlock } from "@/components/ui/Spinner";
import { StatusBadge } from "@/components/recipe/StatusBadge";
import { useCurrentUser } from "@/hooks/useAuth";
import { useRecipes } from "@/hooks/useRecipes";
import { formatDate } from "@/lib/utils";

/**
 * Mục 9: "/dashboard/recipes" — CSR, danh sách công thức CỦA CHÍNH user đang đăng nhập.
 * API GET /recipes không có tài liệu rõ "chỉ trả recipe của tôi", nên ở đây vừa gửi
 * `authorId` (nếu backend hỗ trợ) vừa lọc lại phía client bằng `recipe.author.id` cho chắc.
 */
export default function DashboardRecipesPage() {
  const { user } = useCurrentUser();
  const { data, isLoading, isError, refetch } = useRecipes({
    authorId: user?.id,
    pageSize: 50,
    sort: "-createdAt",
  });

  const myRecipes = (data?.items ?? []).filter((r) => r.author.id === user?.id);

  return (
    <div className="flex flex-col gap-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-neutral-900">Công thức của tôi</h1>
        <Link href="/dashboard/recipes/new">
          <Button>
            <FilePlus2 className="mr-2 h-4 w-4" />
            Tạo công thức mới
          </Button>
        </Link>
      </div>

      {isLoading && <LoadingBlock />}
      {isError && <ErrorBlock message="Không tải được danh sách công thức." onRetry={() => refetch()} />}

      {!isLoading && !isError && myRecipes.length === 0 && (
        <div className="rounded-card border border-dashed border-neutral-300 py-16 text-center text-neutral-500">
          Bạn chưa có công thức nào. Bấm &quot;Tạo công thức mới&quot; để bắt đầu.
        </div>
      )}

      {!isLoading && !isError && myRecipes.length > 0 && (
        <div className="overflow-x-auto rounded-card border border-neutral-200">
          <table className="w-full text-left text-sm">
            <thead className="border-b border-neutral-200 bg-neutral-50 text-neutral-500">
              <tr>
                <th className="px-4 py-3 font-medium">Tên công thức</th>
                <th className="px-4 py-3 font-medium">Danh mục</th>
                <th className="px-4 py-3 font-medium">Trạng thái</th>
                <th className="px-4 py-3 font-medium">Ngày tạo</th>
                <th className="px-4 py-3 font-medium" />
              </tr>
            </thead>
            <tbody className="divide-y divide-neutral-100">
              {myRecipes.map((recipe) => (
                <tr key={recipe.id}>
                  <td className="px-4 py-3 font-medium text-neutral-900">{recipe.title}</td>
                  <td className="px-4 py-3 text-neutral-600">{recipe.category.name}</td>
                  <td className="px-4 py-3">
                    <StatusBadge status={recipe.status} />
                  </td>
                  <td className="px-4 py-3 text-neutral-500">{formatDate(recipe.createdAt)}</td>
                  <td className="px-4 py-3 text-right">
                    <Link
                      href={`/dashboard/recipes/${recipe.slug}/edit`}
                      className="inline-flex items-center gap-1 text-sm font-medium text-brand-700 hover:underline"
                    >
                      <PenSquare className="h-4 w-4" />
                      Sửa
                    </Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
