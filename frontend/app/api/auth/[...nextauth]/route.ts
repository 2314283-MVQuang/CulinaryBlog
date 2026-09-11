import { handlers } from "@/auth";

// Route handler chuẩn của Auth.js v5 — xử lý toàn bộ /api/auth/* (signin, callback, signout, session...).
// Không cần sửa file này, mọi cấu hình thật sự nằm ở auth.ts tại thư mục gốc.
export const { GET, POST } = handlers;
