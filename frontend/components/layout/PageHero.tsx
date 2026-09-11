import { cn } from "@/lib/utils";

interface PageHeroProps {
  title: string;
  description?: string;
  /** Emoji trang trí bay lơ lửng ở hai bên. */
  emojis?: string[];
  /** Nội dung phụ hiện bên dưới mô tả (vd. số lượng kết quả, nút hành động). */
  children?: React.ReactNode;
  className?: string;
}

/**
 * Dải tiêu đề gradient dùng cho các trang bên trong (/recipes, /categories, /search...).
 *
 * Vì sao tách riêng: mọi trang dùng chung một khối này thì giao diện đồng bộ, và sau này
 * muốn đổi phong cách tiêu đề chỉ cần sửa đúng một file.
 *
 * Lưu ý: Header trong suốt khi ở trang chủ; các trang khác Header có nền trắng, nên khối
 * này chừa sẵn khoảng đệm trên (`pt-10`) để không bị Header che.
 */
export function PageHero({ title, description, emojis = [], children, className }: PageHeroProps) {
  return (
    <section className={cn("relative overflow-hidden bg-brand-gradient", className)}>
      {/* Hoa văn chấm bi */}
      <div
        aria-hidden
        className="absolute inset-0 opacity-[0.14]"
        style={{
          backgroundImage: "radial-gradient(#ffffff 1.5px, transparent 1.5px)",
          backgroundSize: "24px 24px",
        }}
      />
      <div aria-hidden className="absolute -right-16 -top-16 h-56 w-56 rounded-full bg-amber-300/30 blur-3xl" />

      {/* Emoji trang trí — ẩn trên màn hình nhỏ để không chen chỗ chữ */}
      {emojis.map((emoji, index) => (
        <span
          key={emoji}
          aria-hidden
          className={cn(
            "absolute hidden text-5xl opacity-45 lg:block",
            index % 2 === 0 ? "animate-float" : "animate-float-slow",
          )}
          style={{
            right: `${4 + index * 7}rem`,
            top: index % 2 === 0 ? "2.5rem" : "auto",
            bottom: index % 2 === 0 ? "auto" : "3rem",
            animationDelay: `${index * 600}ms`,
          }}
        >
          {emoji}
        </span>
      ))}

      <div className="container-page relative py-12 sm:py-16">
        <h1 className="animate-fade-up text-3xl font-bold text-white sm:text-4xl">{title}</h1>
        {description && (
          <p
            className="mt-3 max-w-xl animate-fade-up text-sm leading-relaxed text-brand-50 sm:text-base"
            style={{ animationDelay: "100ms" }}
          >
            {description}
          </p>
        )}
        {children && (
          <div className="mt-5 animate-fade-up" style={{ animationDelay: "180ms" }}>
            {children}
          </div>
        )}
      </div>

      {/* Đường lượn sóng nối xuống nội dung */}
      <svg
        aria-hidden
        viewBox="0 0 1440 60"
        preserveAspectRatio="none"
        className="block h-8 w-full sm:h-14"
        fill="#fffdfa"
      >
        <path d="M0 60 L0 26 C 280 60 560 6 860 22 C 1120 36 1300 56 1440 30 L1440 60 Z" />
      </svg>
    </section>
  );
}
