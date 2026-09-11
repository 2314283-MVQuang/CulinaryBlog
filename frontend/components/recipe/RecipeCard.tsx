import Image from "next/image";
import Link from "next/link";
import { ArrowUpRight, Clock, Users } from "lucide-react";
import { Card } from "@/components/ui/Card";
import { TiltCard } from "@/components/ui/TiltCard";
import { DifficultyBadge } from "./DifficultyBadge";
import { formatMinutes } from "@/lib/utils";
import type { RecipeSummary } from "@/types/recipe";

/**
 * Card hiển thị 1 recipe trong lưới danh sách (trang chủ, /recipes, /categories/[slug], /search).
 *
 * Các hiệu ứng khi rê chuột (đều dùng `group-hover:` của Tailwind, không cần JS):
 *  - Cả card nghiêng nhẹ theo con trỏ + có vầng sáng bám theo (component TiltCard).
 *  - Ảnh phóng to chậm và sáng lên.
 *  - Lớp phủ tối từ dưới đậm dần để chữ trên ảnh luôn đọc được.
 *  - Tiêu đề đổi sang màu cam, mũi tên trượt chéo lên.
 */
export function RecipeCard({ recipe }: { recipe: RecipeSummary }) {
  const totalTime = recipe.prepTime + recipe.cookTime;

  return (
    <TiltCard maxTilt={7} className="h-full">
      <Link href={`/recipes/${recipe.slug}`} className="block h-full">
        <Card className="group flex h-full flex-col overflow-hidden">
          {/* ----- Ảnh ----- */}
          <div className="relative aspect-[4/3] w-full overflow-hidden bg-brand-50">
            {recipe.primaryImageUrl ? (
              <Image
                src={recipe.primaryImageUrl}
                alt={recipe.title}
                fill
                sizes="(min-width: 1024px) 25vw, (min-width: 640px) 33vw, 100vw"
                className="object-cover transition-transform duration-700 ease-springy group-hover:scale-110"
              />
            ) : (
              // Không có ảnh thì vẫn phải đẹp: nền gradient ấm + emoji nồi.
              <div className="flex h-full w-full flex-col items-center justify-center gap-1 bg-warm-gradient">
                <span className="text-4xl transition-transform duration-500 ease-springy group-hover:scale-125">
                  🍲
                </span>
                <span className="text-xs font-medium text-brand-400">Chưa có ảnh</span>
              </div>
            )}

            {/* Lớp phủ tối dần từ dưới lên */}
            <div className="pointer-events-none absolute inset-0 bg-gradient-to-t from-neutral-900/70 via-neutral-900/10 to-transparent opacity-60 transition-opacity duration-500 group-hover:opacity-90" />

            {/* Nhãn độ khó */}
            <div className="absolute left-3 top-3 animate-pop">
              <DifficultyBadge difficulty={recipe.difficulty} />
            </div>

            {/* Nhãn danh mục kiểu kính mờ */}
            <span className="absolute right-3 top-3 rounded-full border border-white/40 bg-white/80 px-2.5 py-1 text-[11px] font-semibold uppercase tracking-wide text-brand-700 backdrop-blur-md">
              {recipe.category.name}
            </span>

            {/* Thời gian nấu — trượt lên khi hover */}
            <span className="absolute bottom-3 left-3 flex translate-y-2 items-center gap-1.5 rounded-full bg-white/90 px-3 py-1.5 text-xs font-semibold text-neutral-800 opacity-0 shadow-soft backdrop-blur-md transition-all duration-500 ease-springy group-hover:translate-y-0 group-hover:opacity-100">
              <Clock className="h-3.5 w-3.5 text-brand-600" />
              {formatMinutes(totalTime)}
            </span>
          </div>

          {/* ----- Nội dung ----- */}
          <div className="flex flex-1 flex-col gap-2 p-4">
            <h3 className="line-clamp-2 font-display text-base font-bold leading-snug text-neutral-900 transition-colors duration-300 group-hover:text-brand-700">
              {recipe.title}
            </h3>
            <p className="line-clamp-2 text-sm leading-relaxed text-neutral-500">{recipe.description}</p>

            {/* Đường kẻ gradient nở ra khi hover */}
            <span className="mt-auto block h-px w-8 bg-brand-gradient transition-all duration-500 ease-springy group-hover:w-full" />

            <div className="flex items-center justify-between pt-1 text-xs text-neutral-500">
              <span className="flex items-center gap-3">
                <span className="flex items-center gap-1">
                  <Clock className="h-3.5 w-3.5 text-brand-500" />
                  {formatMinutes(totalTime)}
                </span>
                <span className="flex items-center gap-1">
                  <Users className="h-3.5 w-3.5 text-brand-500" />
                  {recipe.servings} phần
                </span>
              </span>

              <ArrowUpRight className="h-4 w-4 text-brand-500 opacity-0 transition-all duration-300 ease-springy group-hover:translate-x-0.5 group-hover:-translate-y-0.5 group-hover:opacity-100" />
            </div>
          </div>
        </Card>
      </Link>
    </TiltCard>
  );
}
