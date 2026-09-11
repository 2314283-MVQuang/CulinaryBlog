"use client";

import { useEffect, useRef, useState } from "react";

interface CountUpProps {
  /** Số đích cần đếm tới. */
  to: number;
  /** Thời lượng đếm (ms). */
  duration?: number;
  suffix?: string;
  className?: string;
}

/**
 * Con số chạy từ 0 lên giá trị đích khi cuộn tới — dùng cho khu "thống kê" ở trang chủ.
 *
 * Dùng requestAnimationFrame + hàm easing (easeOutCubic) để số chạy nhanh lúc đầu rồi
 * chậm dần về đích, cảm giác tự nhiên hơn là chạy đều.
 */
export function CountUp({ to, duration = 1600, suffix = "", className }: CountUpProps) {
  const ref = useRef<HTMLSpanElement>(null);
  const [value, setValue] = useState(0);
  const started = useRef(false);

  useEffect(() => {
    const el = ref.current;
    if (!el || typeof IntersectionObserver === "undefined") {
      setValue(to);
      return;
    }

    const observer = new IntersectionObserver(
      (entries) => {
        for (const entry of entries) {
          if (!entry.isIntersecting || started.current) continue;
          started.current = true;
          observer.disconnect();

          const start = performance.now();
          const tick = (now: number) => {
            const progress = Math.min(1, (now - start) / duration);
            // easeOutCubic: nhanh → chậm dần
            const eased = 1 - Math.pow(1 - progress, 3);
            setValue(Math.round(to * eased));
            if (progress < 1) requestAnimationFrame(tick);
          };
          requestAnimationFrame(tick);
        }
      },
      { threshold: 0.4 },
    );

    observer.observe(el);
    return () => observer.disconnect();
  }, [to, duration]);

  return (
    <span ref={ref} className={className}>
      {value.toLocaleString("vi-VN")}
      {suffix}
    </span>
  );
}
