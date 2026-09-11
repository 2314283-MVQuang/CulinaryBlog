import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

/**
 * Gộp class Tailwind an toàn, tự loại bỏ xung đột (vd. "p-2 p-4" -> chỉ giữ "p-4").
 * Dùng ở MỌI component nhận prop `className` thay vì nối chuỗi bằng tay.
 */
export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

/** Format phút thành dạng "1h 30p" / "45p" cho dễ đọc — dùng ở RecipeCard, RecipeDetail. */
export function formatMinutes(minutes: number): string {
  if (minutes <= 0) return "0p";
  const hours = Math.floor(minutes / 60);
  const rest = minutes % 60;
  if (hours === 0) return `${rest}p`;
  if (rest === 0) return `${hours}h`;
  return `${hours}h ${rest}p`;
}

/** Format ngày ISO -> "dd/MM/yyyy" theo locale Việt Nam. */
export function formatDate(iso: string | null | undefined): string {
  if (!iso) return "";
  return new Date(iso).toLocaleDateString("vi-VN", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  });
}

/** Map enum RecipeDifficulty sang nhãn tiếng Việt hiển thị trên UI. */
export const DIFFICULTY_LABEL: Record<string, string> = {
  Easy: "Dễ",
  Medium: "Trung bình",
  Hard: "Khó",
  Expert: "Chuyên gia",
};

/** Map enum RecipeStatus sang nhãn + màu badge tương ứng, dùng trong dashboard. */
export const STATUS_LABEL: Record<string, { label: string; className: string }> = {
  Draft: { label: "Bản nháp", className: "border-neutral-200 bg-neutral-100 text-neutral-600" },
  Published: { label: "Đã đăng", className: "border-herb-200 bg-herb-50 text-herb-700" },
  Archived: { label: "Đã lưu trữ", className: "border-amber-200 bg-amber-50 text-amber-700" },
};
