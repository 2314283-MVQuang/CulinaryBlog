import { z } from "zod";

/**
 * Quy tắc password khớp CONS-004 / mục 5.2:
 * tối thiểu 8 ký tự, có ít nhất 1 chữ hoa, 1 số, 1 ký tự đặc biệt.
 */
const passwordSchema = z
  .string()
  .min(8, "Mật khẩu phải có ít nhất 8 ký tự")
  .regex(/[A-Z]/, "Mật khẩu phải có ít nhất 1 chữ hoa")
  .regex(/[0-9]/, "Mật khẩu phải có ít nhất 1 chữ số")
  .regex(/[^A-Za-z0-9]/, "Mật khẩu phải có ít nhất 1 ký tự đặc biệt");

/** Khớp body POST /auth/register (FR-AUTH-001). */
export const registerSchema = z.object({
  fullName: z.string().min(1, "Vui lòng nhập họ tên"),
  email: z.string().email("Email không hợp lệ"),
  userName: z
    .string()
    .min(3, "Tên đăng nhập tối thiểu 3 ký tự")
    .regex(/^[a-zA-Z0-9_]+$/, "Tên đăng nhập chỉ gồm chữ, số và dấu gạch dưới"),
  password: passwordSchema,
});
export type RegisterFormValues = z.infer<typeof registerSchema>;

/** Khớp body POST /auth/login (FR-AUTH-002). */
export const loginSchema = z.object({
  email: z.string().email("Email không hợp lệ"),
  password: z.string().min(1, "Vui lòng nhập mật khẩu"),
});
export type LoginFormValues = z.infer<typeof loginSchema>;

/** Khớp body PATCH /auth/me (FR-AUTH-007). */
export const updateProfileSchema = z.object({
  displayName: z.string().min(1, "Vui lòng nhập tên hiển thị"),
  bio: z.string().max(500, "Giới thiệu tối đa 500 ký tự").optional(),
  avatarUrl: z.string().url("URL ảnh không hợp lệ").optional().or(z.literal("")),
});
export type UpdateProfileFormValues = z.infer<typeof updateProfileSchema>;
