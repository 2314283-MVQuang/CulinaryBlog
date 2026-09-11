"use client";

import { useEffect, useRef } from "react";

/**
 * Vầng sáng cam mềm bám theo con trỏ chuột trên toàn trang.
 *
 * Đây là hiệu ứng "trang trí", nên:
 *  - `pointer-events-none`: không chắn click của người dùng.
 *  - Ẩn trên thiết bị cảm ứng (không có chuột thì vầng sáng đứng im rất vô duyên) —
 *    kiểm tra bằng media query `(hover: hover)`.
 *  - Tắt khi người dùng bật "giảm chuyển động" trong cài đặt hệ điều hành.
 *
 * Ngoài ra component còn ghi toạ độ chuột vào biến CSS `--cursor-x/--cursor-y` ở thẻ
 * <html>, để bất kỳ chỗ nào trong app cũng dùng được (xem class `.cursor-spotlight`).
 */
export function CursorGlow() {
  const dotRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const fine = window.matchMedia("(hover: hover) and (pointer: fine)").matches;
    const reduced = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    if (!fine || reduced) return;

    let frame = 0;
    let x = 0;
    let y = 0;

    function render() {
      frame = 0;
      const el = dotRef.current;
      if (el) {
        el.style.opacity = "1";
        el.style.transform = `translate3d(${x}px, ${y}px, 0)`;
      }
      document.documentElement.style.setProperty("--cursor-x", `${x}px`);
      document.documentElement.style.setProperty("--cursor-y", `${y}px`);
    }

    function onMove(e: MouseEvent) {
      x = e.clientX;
      y = e.clientY;
      if (frame === 0) frame = requestAnimationFrame(render);
    }

    window.addEventListener("mousemove", onMove, { passive: true });
    return () => {
      window.removeEventListener("mousemove", onMove);
      if (frame !== 0) cancelAnimationFrame(frame);
    };
  }, []);

  return (
    <div
      ref={dotRef}
      aria-hidden
      className="pointer-events-none fixed left-0 top-0 z-[55] hidden h-[420px] w-[420px] opacity-0 mix-blend-multiply transition-opacity duration-500 md:block"
      style={{
        // Lùi nửa kích thước để TÂM vầng sáng trùng đúng đầu con trỏ.
        // (Dùng margin chứ không dùng class -translate-x-1/2 vì `transform` đã bị
        //  translate3d ở trên chiếm chỗ — hai thứ không cộng dồn được.)
        marginLeft: "-210px",
        marginTop: "-210px",
        background:
          "radial-gradient(circle, rgba(253,186,116,0.22) 0%, rgba(249,115,22,0.10) 35%, transparent 70%)",
      }}
    />
  );
}
