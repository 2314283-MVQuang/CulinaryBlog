import type { HTMLAttributes } from "react";
import { cn } from "@/lib/utils";

interface CardProps extends HTMLAttributes<HTMLDivElement> {
  /** Bật hiệu ứng nhấc lên + đổ bóng khi rê chuột. Mặc định bật. */
  interactive?: boolean;
}

/**
 * Khung card dùng chung — bo góc + viền ấm + đổ bóng mềm.
 * Dùng cho RecipeCard, CategoryCard, hộp form...
 *
 * Shadow ở đây là tông nâu-cam (`shadow-soft`, `shadow-lift` trong tailwind.config.ts)
 * chứ không phải đen xám mặc định — nhìn hợp chủ đề ẩm thực và bớt "lạnh" hơn.
 */
export function Card({ className, interactive = true, ...props }: CardProps) {
  return (
    <div
      className={cn(
        "rounded-card border border-brand-100/70 bg-white shadow-soft",
        interactive && "transition-all duration-300 ease-springy hover:border-brand-200 hover:shadow-lift",
        className,
      )}
      {...props}
    />
  );
}
