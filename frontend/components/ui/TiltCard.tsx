"use client";

import { useRef, useState, type ReactNode } from "react";
import { cn } from "@/lib/utils";

interface TiltCardProps {
  children: ReactNode;
  /** Góc nghiêng tối đa (độ). Để nhỏ (6–12) cho tinh tế, lớn hơn sẽ thành "đồ chơi". */
  maxTilt?: number;
  /** Có hiện vầng sáng bám theo chuột bên trong card không. */
  glare?: boolean;
  className?: string;
}

/**
 * Card nghiêng 3D theo vị trí con trỏ chuột + vầng sáng bám theo chuột.
 *
 * Vì sao viết tay thay vì cài thư viện: hiệu ứng này chỉ cần vài phép toán đơn giản,
 * cài thêm package sẽ làm nặng bundle mà không được lợi gì.
 *
 * Cách tính: lấy vị trí chuột trong khung card, đổi về khoảng -0.5 → 0.5 (tâm là 0),
 * rồi nhân với `maxTilt` để ra góc xoay. Chuột ở mép phải → xoay quanh trục Y dương...
 *
 * Lưu ý hiệu năng: ta ghi thẳng vào style (transform) thay vì setState mỗi lần chuột
 * di chuyển — nếu setState liên tục, React sẽ render lại hàng chục lần mỗi giây gây giật.
 */
export function TiltCard({ children, maxTilt = 8, glare = true, className }: TiltCardProps) {
  const ref = useRef<HTMLDivElement>(null);
  const [hovering, setHovering] = useState(false);

  function handleMouseMove(e: React.MouseEvent<HTMLDivElement>) {
    const el = ref.current;
    if (!el) return;

    const rect = el.getBoundingClientRect();
    // px: 0 (mép trái) → 1 (mép phải); py: 0 (mép trên) → 1 (mép dưới)
    const px = (e.clientX - rect.left) / rect.width;
    const py = (e.clientY - rect.top) / rect.height;

    const rotateY = (px - 0.5) * 2 * maxTilt;
    // Trục X ngược dấu: chuột xuống dưới thì card ngửa ra sau.
    const rotateX = -(py - 0.5) * 2 * maxTilt;

    el.style.transform = `perspective(900px) rotateX(${rotateX.toFixed(2)}deg) rotateY(${rotateY.toFixed(
      2,
    )}deg) scale(1.02)`;
    // Vị trí vầng sáng, đơn vị %.
    el.style.setProperty("--glare-x", `${(px * 100).toFixed(1)}%`);
    el.style.setProperty("--glare-y", `${(py * 100).toFixed(1)}%`);
  }

  function handleMouseLeave() {
    const el = ref.current;
    setHovering(false);
    if (!el) return;
    // Trả về vị trí phẳng ban đầu — transition ở className lo phần chuyển động mượt.
    el.style.transform = "perspective(900px) rotateX(0deg) rotateY(0deg) scale(1)";
  }

  return (
    <div
      ref={ref}
      onMouseMove={handleMouseMove}
      onMouseEnter={() => setHovering(true)}
      onMouseLeave={handleMouseLeave}
      className={cn("gpu relative transition-transform duration-300 ease-springy", className)}
    >
      {children}

      {glare && (
        <span
          aria-hidden
          className={cn(
            "pointer-events-none absolute inset-0 z-20 rounded-[inherit] transition-opacity duration-300",
            hovering ? "opacity-100" : "opacity-0",
          )}
          style={{
            background:
              "radial-gradient(240px circle at var(--glare-x, 50%) var(--glare-y, 50%), rgba(255,255,255,0.35), transparent 60%)",
          }}
        />
      )}
    </div>
  );
}
