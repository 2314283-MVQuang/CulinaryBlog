
# 🍳 PHÂN CÔNG CÔNG VIỆC — DỰ ÁN CULINARYBLOG

## 🎯 LAB 2: HOÀN THIỆN CHỨC NĂNG ĐĂNG NHẬP (16/9/2026 - 22/9/2026)

| Người phụ trách | Mã FR | Tính năng |
| :--- | :--- | :--- |
| **Ý** | `FR-AUTH-003` | Đăng nhập bằng Google (Google OAuth 2.0) |
| **Tiến** | `FR-OBS-003` | Rate limiting cho đăng nhập/đăng ký |
| **Bảo Thịnh** | `FR-JOB-001` | Gửi email chào mừng ở tác vụ nền |
| **Quang** | `FR-AUTH-007` | PATCH `/api/v1/auth/me` (cập nhật hồ sơ: DisplayName, AvatarUrl, Bio) |

---

## 📋 CHI TIẾT PHÂN CÔNG CÔNG VIỆC TOÀN DỰ ÁN

| STT | Mã FR | Chức năng | Người phụ trách | Trạng thái / Thời gian |
| :---: | :--- | :--- | :---: | :---: |
| **1** | **FR-AUTH** | **Xác thực & Người dùng (7 FR)** | **—** | **Xong hết** |
| 1 | FR-AUTH-001 | Đăng ký tài khoản | — | Đã xong |
| 2 | FR-AUTH-002 | Đăng nhập email/mật khẩu | — | Đã xong |
| 3 | FR-AUTH-003 | Đăng nhập Google OAuth | — | Chưa xong |
| 4 | FR-AUTH-004 | Làm mới Access Token (refresh + rotation) | — | Đã xong |
| 5 | FR-AUTH-005 | Đăng xuất | — | Đã xong |
| 6 | FR-AUTH-006 | Xem hồ sơ cá nhân (GET /auth/me) | — | Đã xong |
| 7 | FR-AUTH-007 | Cập nhật hồ sơ cá nhân (PATCH /auth/me) | — | Đã xong |
| **2** | **FR-CAT** | **Quản lý Danh mục (5 FR)** | **Ý** | **—** |
| 8 | FR-CAT-001 | Xem danh sách danh mục | Ý | Tuần 3 |
| 9 | FR-CAT-002 | Xem chi tiết danh mục (kèm recipe) | Ý | Tuần 3 |
| 10 | FR-CAT-003 | Tạo danh mục mới | Ý | Tuần 4 |
| 11 | FR-CAT-004 | Cập nhật danh mục | Ý | Tuần 4 |
| 12 | FR-CAT-005 | Xóa danh mục | Ý | Tuần 5 |
| **3** | **FR-RCP** | **Quản lý Công thức (10 FR)** | **Quang** | **—** |
| 13 | FR-RCP-001 | Danh sách công thức (phân trang/lọc/sắp xếp) | Quang | Tuần 3 |
| 14 | FR-RCP-002 | Chi tiết công thức | Quang | Tuần 3 |
| 15 | FR-RCP-003 | Tạo công thức mới | Quang | Tuần 4 |
| 16 | FR-RCP-004 | Cập nhật thông tin cơ bản | Quang | Tuần 3 |
| 17 | FR-RCP-005 | Publish công thức | Quang | Tuần 4 |
| 18 | FR-RCP-006 | Hủy publish / Lưu trữ (Archive) | Quang | Tuần 4 |
| 19 | FR-RCP-007 | Xóa công thức (hard delete) | Quang | Tuần 4 |
| 20 | FR-RCP-008 | Quản lý ảnh công thức | Quang | Tuần 4 |
| 21 | FR-RCP-009 | Quản lý nguyên liệu | Quang | Tuần 5 |
| 22 | FR-RCP-010 | Quản lý các bước thực hiện | Quang | Tuần 5 |
| **4** | **FR-SRCH** | **Tìm kiếm & Phân trang (4 FR)** | **Thịnh** | **—** |
| 23 | FR-SRCH-001 | Tìm kiếm toàn văn bản | Thịnh | Tuần 3 |
| 24 | FR-SRCH-002 | Lọc công thức | Thịnh | Tuần 3 |
| 25 | FR-SRCH-003 | Sắp xếp kết quả (sort=-field) | Thịnh | Tuần 4 |
| 26 | FR-SRCH-004 | Phân trang | Thịnh | Tuần 4 |
| **5** | **FR-FILE** | **Quản lý Tệp tin (2 FR)** | **Tiến** | **—** |
| 27 | FR-FILE-001 | Upload file lên MinIO | Tiến | Tuần 3 |
| 28 | FR-FILE-002 | Xóa file khỏi MinIO | Tiến | Tuần 3 |
| **6** | **FR-JOB** | **Background Jobs (3 FR)** | **Tiến** | **—** |
| 29 | FR-JOB-001 | Welcome Email (Hangfire) | Tiến | Tuần 4 |
| 30 | FR-JOB-002 | Sinh Thumbnail | Tiến | Tuần 4 |
| 31 | FR-JOB-003 | Sinh Sitemap | Tiến | Tuần 5 |
| **7** | **FR-OBS** | **Quan sát Hệ thống (2 FR)** | **Thịnh, Ý** | **—** |
| 32 | FR-OBS-001 | Health Check Endpoints | Ý | Ý — Tuần 5 |
| 33 | FR-OBS-002 | Structured Logging (Serilog) | Thịnh | Thịnh — Tuần 5 |
