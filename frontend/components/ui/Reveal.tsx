"use client";

import { useEffect, useRef, useState, type ReactNode } from "react";
import { cn } from "@/lib/utils";

interface RevealProps {
  children: ReactNode;
  /** Độ trễ (ms) trước khi hiện — dùng để các phần tử trong lưới hiện lần lượt. */
  delay?: number;
  /** Hướng trượt vào: lên (mặc định), trái, phải, hoặc chỉ mờ dần. */
  direction?: "up" | "left" | "right" | "none";
  className?: string;
}

const HIDDEN_TRANSFORM: Record<NonNullable<RevealProps["direction"]>, string> = {
  up: "translate-y-8",
  left: "-translate-x-8",
  right: "translate-x-8",
  none: "",
};

/**
 * Bọc bất kỳ nội dung nào để nó "hiện dần + trượt vào" khi cuộn tới.
 *
 * Cách hoạt động: dùng IntersectionObserver — API sẵn có của trình duyệt, báo cho ta biết
 * khi phần tử lọt vào vùng nhìn thấy. So với việc lắng nghe sự kiện scroll thủ công thì
 * cách này nhẹ hơn nhiều vì trình duyệt tự tối ưu.
 *
 * Dùng: <Reveal delay={100}><RecipeCard .../></Reveal>
 */
export function Reveal({ children, delay = 0, direction = "up", className }: RevealProps) {
  const ref = useRef<HTMLDivElement>(null);
  const [visible, setVisible] = useState(false);

  useEffect(() => {
    const el = ref.current;
    if (!el) return;

    // Trình duyệt quá cũ không có IntersectionObserver → hiện luôn, không animate.
    if (typeof IntersectionObserver === "undefined") {
      setVisible(true);
      return;
    }

    const observer = new IntersectionObserver(
      (entries) => {
        for (const entry of entries) {
          if (entry.isIntersecting) {
            setVisible(true);
            // Chỉ animate 1 lần duy nhất — cuộn lên cuộn xuống không lặp lại cho đỡ rối mắt.
            observer.unobserve(entry.target);
          }
        }
      },
      // rootMargin âm ở dưới: đợi phần tử vào sâu trong màn hình ~12% rồi mới chạy.
      { threshold: 0.1, rootMargin: "0px 0px -12% 0px" },
    );

    observer.observe(el);
    return () => observer.disconnect();
  }, []);

  return (
    <div
      ref={ref}
      style={{ transitionDelay: `${delay}ms` }}
      className={cn(
        "transition-all duration-700 ease-springy",
        visible ? "translate-x-0 translate-y-0 opacity-100" : cn("opacity-0", HIDDEN_TRANSFORM[direction]),
        className,
      )}
    >
      {children}
    </div>
  );
}
