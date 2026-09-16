"use client";

import Link from "next/link";
import { FilePlus2, PenSquare } from "lucide-react";
import { Button } from "@/components/ui/Button";
import { ErrorBlock, LoadingBlock } from "@/components/ui/Spinner";
import { StatusBadge } from "@/components/recipe/StatusBadge";
import { useRecipes } from "@/hooks/useRecipes";
import { formatDate } from "@/lib/utils";

/**
 * Mục 9: "/dashboard/recipes" — CSR, danh sách công thức để vào trang chỉnh sửa.
 *
 * GIỚI HẠN HIỆN TẠI của GET /recipes (FR-RCP-001, còn TODO ở backend): endpoint chỉ trả
 * Status=Published, không nhận tham số lọc theo tác giả, và RecipeListItemDto không có id tác giả
 * nên cũng không lọc lại phía client được. Vì vậy bảng dưới đây là công thức ĐÃ ĐĂNG của mọi
 * người, và công thức nháp vừa tạo sẽ KHÔNG xuất hiện ở đây — sau khi tạo xong, trình tạo công
 * thức chuyển thẳng sang trang sửa nên vẫn dùng được.
 */
export default function DashboardRecipesPage() {
  const { data, isLoading, isError, refetch } = useRecipes({
    pageSize: 50,
    sort: "-createdAt",
  });

  const myRecipes = data?.items ?? [];

  // Tiêu đề cột và tên biến giữ nguyên "công thức của tôi" để khỏi phải sửa lại khi backend bổ
  // sung bộ lọc tác giả; phần thông báo dưới đây nói rõ dữ liệu hiện tại chưa được lọc.

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

      <p className="rounded-card border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-800">
        Bảng này đang liệt kê công thức <strong>đã đăng của mọi người</strong>, chưa lọc theo tác
        giả — API danh sách (FR-RCP-001) chưa hỗ trợ lọc theo tác giả và cũng chưa trả về công
        thức nháp. Công thức nháp bạn vừa tạo sẽ không hiện ở đây; bấm &quot;Tạo công thức
        mới&quot; sẽ chuyển thẳng sang trang chỉnh sửa của nó.
      </p>

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
                <th className="px-4 py-3 font-medium">Ngày đăng</th>
                <th className="px-4 py-3 font-medium" />
              </tr>
            </thead>
            <tbody className="divide-y divide-neutral-100">
              {myRecipes.map((recipe) => (
                <tr key={recipe.id}>
                  <td className="px-4 py-3 font-medium text-neutral-900">{recipe.title}</td>
                  <td className="px-4 py-3 text-neutral-600">{recipe.categoryName}</td>
                  <td className="px-4 py-3">
                    <StatusBadge status={recipe.status} />
                  </td>
                  <td className="px-4 py-3 text-neutral-500">{formatDate(recipe.publishedAt) || "—"}</td>
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
