"use client";

import { useEffect, useRef } from "react";

/**
 * Thanh gradient mảnh chạy ngang trên đỉnh trang, dài dần theo mức độ cuộn.
 * Đặt 1 lần trong app/layout.tsx là có ở mọi trang.
 *
 * Hiệu năng: cập nhật trực tiếp `style.transform` của thẻ div thay vì setState,
 * và bọc trong requestAnimationFrame để mỗi khung hình chỉ vẽ đúng 1 lần.
 */
export function ScrollProgress() {
  const barRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    let frame = 0;

    function update() {
      frame = 0;
      const el = barRef.current;
      if (!el) return;

      const scrollable = document.documentElement.scrollHeight - window.innerHeight;
      // Trang ngắn hơn màn hình thì không có gì để cuộn → coi như 0%.
      const ratio = scrollable > 0 ? window.scrollY / scrollable : 0;
      el.style.transform = `scaleX(${Math.min(1, Math.max(0, ratio))})`;
    }

    function onScroll() {
      // Gộp nhiều sự kiện scroll dồn dập thành 1 lần vẽ.
      if (frame === 0) frame = requestAnimationFrame(update);
    }

    update();
    window.addEventListener("scroll", onScroll, { passive: true });
    window.addEventListener("resize", onScroll);
    return () => {
      window.removeEventListener("scroll", onScroll);
      window.removeEventListener("resize", onScroll);
      if (frame !== 0) cancelAnimationFrame(frame);
    };
  }, []);

  return (
    <div aria-hidden className="fixed inset-x-0 top-0 z-[60] h-1 bg-transparent">
      <div
        ref={barRef}
        className="h-full origin-left scale-x-0 bg-brand-gradient shadow-[0_0_12px_rgba(249,115,22,0.7)]"
      />
    </div>
  );
}
