import Image from "next/image";
import Link from "next/link";
import { ArrowUpRight } from "lucide-react";
import { Card } from "@/components/ui/Card";
import { TiltCard } from "@/components/ui/TiltCard";
import type { Category } from "@/types/category";

/**
 * Emoji mặc định cho danh mục chưa có ảnh — chọn theo tên để mỗi danh mục
 * có biểu tượng riêng thay vì tất cả đều giống nhau.
 */
const EMOJI_BY_KEYWORD: Array<[RegExp, string]> = [
  [/khai vị|salad|gỏi/i, "🥗"],
  [/tráng miệng|bánh|ngọt|chè/i, "🍰"],
  [/súp|canh|lẩu/i, "🍲"],
  [/mì|phở|bún|noodle/i, "🍜"],
  [/nướng|bbq/i, "🍖"],
  [/hải sản|cá|tôm/i, "🦐"],
  [/chay/i, "🥬"],
  [/đồ uống|nước|sinh tố/i, "🥤"],
  [/sáng|breakfast/i, "🍳"],
];

function pickEmoji(name: string): string {
  for (const [pattern, emoji] of EMOJI_BY_KEYWORD) {
    if (pattern.test(name)) return emoji;
  }
  return "🍽️";
}

/** Card danh mục — dùng ở trang chủ và /categories. */
export function CategoryCard({ category }: { category: Category }) {
  const emoji = pickEmoji(category.name);

  return (
    <TiltCard maxTilt={9} className="h-full">
      <Link href={`/categories/${category.slug}`} className="block h-full">
        <Card className="group relative h-full overflow-hidden">
          <div className="relative aspect-[16/10] w-full overflow-hidden bg-brand-50">
            {category.imageUrl ? (
              <Image
                src={category.imageUrl}
                alt={category.name}
                fill
                sizes="(min-width: 1024px) 25vw, 50vw"
                className="object-cover transition-transform duration-700 ease-springy group-hover:scale-110"
              />
            ) : (
              <div className="flex h-full w-full items-center justify-center bg-warm-gradient">
                <span className="text-5xl transition-transform duration-500 ease-springy group-hover:-rotate-12 group-hover:scale-125">
                  {emoji}
                </span>
              </div>
            )}

            {/* Lớp phủ cam đậm dần khi hover */}
            <div className="pointer-events-none absolute inset-0 bg-brand-gradient opacity-0 mix-blend-multiply transition-opacity duration-500 group-hover:opacity-25" />
          </div>

          <div className="flex items-center justify-between gap-2 p-4">
            <span className="min-w-0">
              <h3 className="truncate font-display text-base font-bold text-neutral-900 transition-colors duration-300 group-hover:text-brand-700">
                {category.name}
              </h3>
              <p className="mt-0.5 text-xs text-neutral-500">{category.recipeCount} công thức</p>
            </span>

            <span className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-brand-50 text-brand-600 transition-all duration-300 ease-springy group-hover:bg-brand-gradient group-hover:text-white">
              <ArrowUpRight className="h-4 w-4" />
            </span>
          </div>
        </Card>
      </Link>
    </TiltCard>
  );
}
