import { type ButtonHTMLAttributes, forwardRef } from "react";
import { cn } from "@/lib/utils";

type Variant = "primary" | "secondary" | "outline" | "ghost" | "danger";
type Size = "sm" | "md" | "lg";

export interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: Variant;
  size?: Size;
  isLoading?: boolean;
}

const variantClasses: Record<Variant, string> = {
  primary: "bg-brand-gradient text-white shadow-soft hover:shadow-glow focus-visible:ring-brand-500",
  secondary: "bg-neutral-900 text-white shadow-soft hover:bg-neutral-800 focus-visible:ring-neutral-700",
  outline: "border border-neutral-300 text-neutral-700 hover:border-brand-300 hover:bg-brand-50 hover:text-brand-700 focus-visible:ring-brand-400",
  ghost: "text-neutral-700 hover:bg-brand-50 hover:text-brand-700 focus-visible:ring-brand-400",
  danger: "bg-spice-600 text-white shadow-soft hover:bg-spice-700 focus-visible:ring-spice-500",
};

const sizeClasses: Record<Size, string> = {
  sm: "h-9 px-3.5 text-sm",
  md: "h-11 px-5 text-sm",
  lg: "h-12 px-6 text-base",
};

/**
 * Button dùng chung cho toàn app. KHÔNG chứa logic nghiệp vụ — chỉ nhận `variant`/`size`
 * và render đúng style. Muốn 1 nút loading khi submit form thì truyền `isLoading`.
 *
 * Hiệu ứng chuột có sẵn:
 *  - Hover: nhấc lên 2px + đổ bóng đậm hơn (`hover:-translate-y-0.5`).
 *  - Nhấn : lún xuống, nảy trở lại (`active:translate-y-0 active:scale-[0.97]`).
 *  - Vệt sáng quét ngang khi hover (class `.shine-on-hover` trong globals.css).
 */
export const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  ({ className, variant = "primary", size = "md", isLoading, disabled, children, ...props }, ref) => {
    return (
      <button
        ref={ref}
        disabled={disabled || isLoading}
        className={cn(
          "shine-on-hover inline-flex items-center justify-center gap-2 rounded-xl font-medium",
          "transition-all duration-300 ease-springy",
          "hover:-translate-y-0.5 active:translate-y-0 active:scale-[0.97]",
          "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-offset-2",
          "disabled:pointer-events-none disabled:cursor-not-allowed disabled:opacity-50",
          variantClasses[variant],
          sizeClasses[size],
          className,
        )}
        {...props}
      >
        {isLoading && (
          <span className="h-4 w-4 animate-spin rounded-full border-2 border-current border-t-transparent" />
        )}
        {children}
      </button>
    );
  },
);
Button.displayName = "Button";
