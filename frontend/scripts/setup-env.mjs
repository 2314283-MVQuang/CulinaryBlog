/**
 * Tự tạo file `.env.local` cho người mới clone project về.
 *
 * Vì sao cần script này:
 *   `.env.local` chứa khoá bí mật nên KHÔNG được commit lên git (xem .gitignore).
 *   Nhưng Auth.js v5 bắt buộc phải có biến AUTH_SECRET — thiếu nó thì endpoint
 *   /api/auth/session trả về HTTP 500 và cả trang báo lỗi ngay khi vừa mở.
 *   Script này chạy tự động trước `npm run dev` (xem "predev" trong package.json),
 *   copy từ .env.local.example và tự sinh một AUTH_SECRET ngẫu nhiên.
 *
 * Script an toàn khi chạy lại nhiều lần: nếu .env.local đã tồn tại thì bỏ qua,
 * không bao giờ ghi đè cấu hình bạn đã chỉnh tay.
 */
import { randomBytes } from "node:crypto";
import { existsSync, readFileSync, writeFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const projectRoot = join(dirname(fileURLToPath(import.meta.url)), "..");
const envPath = join(projectRoot, ".env.local");
const examplePath = join(projectRoot, ".env.local.example");

if (existsSync(envPath)) {
  // Đã có cấu hình riêng — không đụng vào.
  process.exit(0);
}

if (!existsSync(examplePath)) {
  console.warn("[setup-env] Không tìm thấy .env.local.example — bỏ qua bước tạo .env.local.");
  process.exit(0);
}

// 32 byte ngẫu nhiên, mã hoá base64 — đúng chuẩn Auth.js khuyến nghị (giống `npx auth secret`).
const secret = randomBytes(32).toString("base64");

const content = readFileSync(examplePath, "utf8").replace(/^AUTH_SECRET=.*$/m, `AUTH_SECRET=${secret}`);

writeFileSync(envPath, content, "utf8");

console.log("[setup-env] Đã tạo .env.local với AUTH_SECRET ngẫu nhiên.");
console.log("[setup-env] Mở file đó để sửa NEXT_PUBLIC_API_BASE_URL nếu backend chạy ở cổng khác.");
