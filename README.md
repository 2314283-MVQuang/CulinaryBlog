# Culinary Blog

Blog công thức nấu ăn — backend .NET 10 (Clean Architecture + CQRS/MediatR) và frontend Next.js 15.

| Thư mục | Nội dung | README riêng |
|---|---|---|
| `src/` | Backend .NET: Domain, Application, Infrastructure, API | [src/README.md](src/README.md) |
| `frontend/` | Next.js App Router + TypeScript | [frontend/README.md](frontend/README.md) |
| `db/` | Script tạo database PostgreSQL | [db/README.md](db/README.md) |

---

## Chạy lần đầu sau khi `git clone`

Cần có sẵn: **.NET SDK 10**, **Node.js 20+**, và **PostgreSQL 16+** (hoặc Docker Desktop).

### Bước 1 — Dựng database

Chọn **một** trong hai cách.

**Cách A — PostgreSQL đã cài sẵn trên máy.** Tạo database rồi chạy lần lượt 3 file:

```bash
createdb -U postgres CulinaryBlogDb
psql -U postgres -d CulinaryBlogDb -f db/init/01-extensions.sql
psql -U postgres -d CulinaryBlogDb -f db/init/02-schema.sql
psql -U postgres -d CulinaryBlogDb -f db/init/03-seed.sql
```

Dùng pgAdmin cũng được: tạo database `CulinaryBlogDb`, vào *Tools > Query Tool*, mở từng file rồi bấm F5 theo đúng thứ tự trên.

**Cách B — Docker.** Ba file SQL tự chạy ở lần khởi động đầu tiên:

```bash
docker compose up -d postgres
```

Lưu ý: cách B dùng cổng 5432, sẽ đụng nếu máy bạn đã cài sẵn PostgreSQL chạy nền. Dừng service kia trước, hoặc dùng cách A.

### Bước 2 — Khai báo secret cho backend

Secret không nằm trong Git, nên mỗi máy phải tự đặt một lần. Thiếu bước này backend sẽ dừng ngay khi khởi động với thông báo *"Thiếu cấu hình Jwt:Secret"*.

```bash
cd src/CulinaryBlog.API
dotnet user-secrets set "Jwt:Secret" "chuoi-bi-mat-it-nhat-32-ky-tu-doi-truoc-khi-deploy"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=CulinaryBlogDb;Username=postgres;Password=MAT_KHAU_CUA_BAN"
cd ../..
```

Thay `MAT_KHAU_CUA_BAN` bằng mật khẩu PostgreSQL trên máy bạn. Nếu dùng Docker (cách B) thì connection string trong `appsettings.json` đã đúng sẵn, chỉ cần đặt `Jwt:Secret`.

Đừng chạy `dotnet user-secrets init` — `.csproj` đã có sẵn `UserSecretsId`.

### Bước 3 — Chạy backend

```bash
dotnet run --project src/CulinaryBlog.API --launch-profile http
```

Chờ tới dòng `Now listening on: http://localhost:5000`, để nguyên cửa sổ này.

Kiểm tra nhanh: <http://localhost:5000/health> trả `{"status":"healthy"}`, còn <http://localhost:5000/scalar> là giao diện thử API (thay cho Swagger UI).

Mọi endpoint nghiệp vụ đều có tiền tố `/api/v1`. Gõ thiếu sẽ ra 404.

### Bước 4 — Chạy frontend

Mở cửa sổ terminal **thứ hai**:

```bash
cd frontend
npm install
npm run dev
```

Mở <http://localhost:3000>. File `.env.local` được sinh tự động ở lần chạy đầu (`npm run setup` chạy trước `npm run dev`), không cần tạo tay.

---

## Tài khoản mẫu

Có sẵn sau khi chạy `db/init/03-seed.sql`:

| Email | Mật khẩu | Vai trò |
|---|---|---|
| `admin@culinaryblog.local` | `Admin@123` | Admin |
| `author@culinaryblog.local` | `Author@123` | Author |

---

## Lỗi hay gặp

| Hiện tượng | Nguyên nhân và cách xử lý |
|---|---|
| `address already in use` khi chạy backend | Còn tiến trình cũ giữ cổng 5000. Chạy `taskkill /F /IM CulinaryBlog.API.exe` (Windows) rồi mở lại. |
| `Thiếu cấu hình Jwt:Secret` | Chưa làm bước 2, hoặc chạy lệnh `user-secrets` sai thư mục. |
| `/api/v1/categories` trả `{"data":[]}` | Kết nối database được nhưng chưa có dữ liệu — chạy `db/init/03-seed.sql`. |
| Lỗi 500 `__webpack_modules__ is not a function` ở frontend | Đã chạy `npm run build` trong lúc `npm run dev` đang chạy; hai lệnh dùng chung thư mục `.next`. Xoá `.next` rồi chạy lại. |
| Mọi URL đều 404 | Thư mục `src/` chưa có code — `git pull` lại. |
| Trên Windows: `dotnet run` báo lỗi `0x800711C7` | Smart App Control đang chặn file `.dll` tự build. Tắt nó trong *Windows Security > App & browser control*, hoặc chạy backend bằng Docker: `docker compose up --build api`. |

---

## Quy ước làm việc nhóm

- Mỗi người làm theo pattern trong [src/README.md](src/README.md): Validator + Handler + DTO cho mỗi command/query.
- Đặt tên nhánh theo mã FR, ví dụ `feat/FR-AUTH-007-update-profile`.
- Commit message dạng `[FR-XXX] Mô tả ngắn`.
- **Không bao giờ** commit mật khẩu hay JWT secret lên Git — dùng `dotnet user-secrets` (bước 2).
