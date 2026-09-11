import { Badge } from "@/components/ui/Badge";
import { cn, DIFFICULTY_LABEL } from "@/lib/utils";
import type { RecipeDifficulty } from "@/types/recipe";

/** Màu + số chấm tăng dần theo độ khó — nhìn là đoán được mức độ mà không cần đọc chữ. */
const STYLE: Record<RecipeDifficulty, { className: string; dots: number }> = {
  Easy: { className: "border-herb-200 bg-herb-50 text-herb-700", dots: 1 },
  Medium: { className: "border-amber-200 bg-amber-50 text-amber-700", dots: 2 },
  Hard: { className: "border-brand-200 bg-brand-50 text-brand-700", dots: 3 },
  Expert: { className: "border-spice-200 bg-spice-50 text-spice-700", dots: 4 },
};

export function DifficultyBadge({ difficulty }: { difficulty: RecipeDifficulty }) {
  const style = STYLE[difficulty];

  return (
    <Badge className={style.className}>
      {/* Chấm đặc = mức độ đã đạt tới, chấm mờ = chưa. */}
      <span className="flex gap-0.5" aria-hidden>
        {Array.from({ length: 4 }).map((_, i) => (
          <span
            key={i}
            className={cn("h-1.5 w-1.5 rounded-full bg-current", i >= style.dots && "opacity-25")}
          />
        ))}
      </span>
      {DIFFICULTY_LABEL[difficulty]}
    </Badge>
  );
}
