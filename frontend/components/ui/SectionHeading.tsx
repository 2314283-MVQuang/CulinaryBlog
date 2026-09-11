import Link from "next/link";
import { ArrowRight } from "lucide-react";
import { cn } from "@/lib/utils";

interface SectionHeadingProps {
  /** Nhãn nhỏ in hoa phía trên tiêu đề, vd. "Mới nhất". */
  eyebrow?: string;
  title: string;
  description?: string;
  /** Link "Xem tất cả" ở góc phải (bỏ trống thì không hiện). */
  moreHref?: string;
  moreLabel?: string;
  className?: string;
}

/** Cụm tiêu đề dùng lại cho mọi khối nội dung (trang chủ, trang danh sách...). */
export function SectionHeading({
  eyebrow,
  title,
  description,
  moreHref,
  moreLabel = "Xem tất cả",
  className,
}: SectionHeadingProps) {
  return (
    <div className={cn("mb-7 flex flex-wrap items-end justify-between gap-4", className)}>
      <div>
        {eyebrow && (
          <span className="inline-flex items-center gap-2 text-xs font-semibold uppercase tracking-[0.14em] text-brand-600">
            <span className="h-px w-6 bg-brand-400" />
            {eyebrow}
          </span>
        )}
        <h2 className="mt-2 text-2xl font-bold sm:text-3xl">{title}</h2>
        {description && <p className="mt-2 max-w-xl text-sm text-neutral-500 sm:text-base">{description}</p>}
      </div>

      {moreHref && (
        <Link
          href={moreHref}
          className="group inline-flex items-center gap-1.5 rounded-full border border-brand-200 bg-white px-4 py-2 text-sm font-medium text-brand-700 shadow-soft transition-all duration-300 ease-springy hover:-translate-y-0.5 hover:border-brand-300 hover:shadow-lift"
        >
          {moreLabel}
          <ArrowRight className="h-4 w-4 transition-transform duration-300 group-hover:translate-x-1" />
        </Link>
      )}
    </div>
  );
}
