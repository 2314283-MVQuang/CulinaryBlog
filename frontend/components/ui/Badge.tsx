import type { HTMLAttributes } from "react";
import { cn } from "@/lib/utils";

/**
 * Badge nhỏ dùng cho nhãn trạng thái/độ khó.
 * Màu do nơi dùng truyền vào qua `className` — component này không tự quyết định màu,
 * nhờ vậy tái dùng được cho mọi loại nhãn (xem DifficultyBadge, StatusBadge).
 */
export function Badge({ className, ...props }: HTMLAttributes<HTMLSpanElement>) {
  return (
    <span
      className={cn(
        "inline-flex items-center gap-1 rounded-full border px-2.5 py-1 text-[11px] font-semibold",
        "shadow-sm backdrop-blur-sm transition-transform duration-300 ease-springy hover:scale-105",
        className,
      )}
      {...props}
    />
  );
}
