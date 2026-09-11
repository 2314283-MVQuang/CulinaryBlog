# Cơ sở dữ liệu — Culinary Blog

PostgreSQL 16, schema dựng đúng theo **mục 7 (Mô hình dữ liệu)** của tài liệu đặc tả.

| File | Nội dung |
|---|---|
| `init/01-extensions.sql` | Bật `pgcrypto`, `unaccent`, `pg_trgm` và tạo cấu hình tìm kiếm `vietnamese` |
| `init/02-schema.sql` | Tạo toàn bộ bảng, index, ràng buộc và trigger |
| `init/03-seed.sql` | Dữ liệu mẫu: 2 role, 2 tài khoản, 8 danh mục, 3 công thức kèm nguyên liệu và các bước |

Ba file chạy được **nhiều lần không sợ hỏng** — bảng đã có thì bỏ qua, dữ liệu đã có thì không chèn trùng.

---

## Cách 1 — Dùng Docker (khuyến nghị)

Ở thư mục gốc repo:

```bash
docker compose up -d postgres
```

Xong. PostgreSQL tự chạy cả 3 file SQL ngay lần khởi động đầu tiên.

Muốn có giao diện web để xem/sửa dữ liệu mà không phải cài thêm phần mềm:

```bash
docker compose up -d postgres adminer
```

Mở http://localhost:8080 và đăng nhập:

| Trường | Giá trị |
|---|---|
| System | PostgreSQL |
| Server | `postgres` |
| Username | `culinary` |
| Password | `culinary123` |
| Database | `culinaryblog` |

> **Sửa file SQL rồi mà không thấy thay đổi?** Các file trong `init/` chỉ chạy khi
> volume dữ liệu còn trống. Chạy `docker compose down -v` để xoá sạch dữ liệu rồi
> `docker compose up -d postgres` lại.

---

## Cách 2 — Máy đã cài sẵn PostgreSQL

```bash
psql -U postgres -c "CREATE DATABASE culinaryblog;"
psql -U postgres -c "CREATE USER culinary WITH PASSWORD 'culinary123';"
psql -U postgres -c "GRANT ALL PRIVILEGES ON DATABASE culinaryblog TO culinary;"

psql -U postgres -d culinaryblog -f db/init/01-extensions.sql
psql -U postgres -d culinaryblog -f db/init/02-schema.sql
psql -U postgres -d culinaryblog -f db/init/03-seed.sql
```

File `01-extensions.sql` cần quyền tạo extension nên chạy bằng tài khoản `postgres`
(hoặc một superuser khác), hai file còn lại thì tài khoản thường cũng được.

Nếu quen dùng **pgAdmin**: tạo database `culinaryblog`, chuột phải chọn *Query Tool*,
rồi mở lần lượt 3 file theo đúng thứ tự và bấm chạy.

---

## Chuỗi kết nối cho backend .NET

Thêm vào `src/CulinaryBlog.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=culinaryblog;Username=culinary;Password=culinary123"
  }
}
```

---

## Tài khoản mẫu

| Email | Mật khẩu | Role |
|---|---|---|
| `admin@culinaryblog.local` | `Admin@123` | Admin + Author |
| `author@culinaryblog.local` | `Author@123` | Author |

Mật khẩu đã được băm đúng chuẩn PBKDF2 của ASP.NET Core Identity nên đăng nhập được
ngay qua `POST /api/v1/auth/login` khi backend chạy.

> Hai tài khoản này **chỉ để học và test trên máy cá nhân**. Trước khi đưa hệ thống
> lên máy chủ thật, xoá chúng đi hoặc đổi mật khẩu.

---

## Vài điểm đáng chú ý trong schema

**Tìm kiếm không dấu.** Cột `SearchVector` của bảng `Recipes` được một trigger tự cập
nhật mỗi khi `Title` hoặc `Description` thay đổi, dùng cấu hình `vietnamese` có bỏ dấu.
Nhờ vậy gõ `pho bo` vẫn ra `Phở bò Hà Nội`. Tiêu đề được tính điểm cao hơn mô tả khi
xếp hạng kết quả.

**Chống ghi đè lẫn nhau.** Mọi bảng kế thừa `BaseEntity` đều có cột `RowVersion`, được
trigger `touch_row()` đổi giá trị sau mỗi lần UPDATE. Backend đọc recipe ra kèm
`RowVersion`, lúc lưu gửi lại qua header `If-Match`; nếu người khác đã sửa trong lúc
đó thì giá trị không khớp và API trả lỗi `RECIPE_CONCURRENCY_CONFLICT` thay vì âm thầm
ghi đè mất công sức của người kia.

**Không xoá được danh mục đang có công thức.** Khoá ngoại `Recipes.CategoryId` đặt
`ON DELETE RESTRICT` — đúng theo mã lỗi `CATEGORY_DELETE_HAS_RECIPES` trong mục 10.2.

**Mỗi công thức chỉ có đúng một ảnh đại diện.** Ràng buộc này được bảo đảm bằng một
*partial unique index* trên `RecipeImages("RecipeId") WHERE "IsPrimary"`.

---

## Lưu ý quan trọng khi backend bắt đầu dùng EF Core

Tài liệu đặc tả chọn hướng **EF Core Code First** (mục 7): schema thật sẽ do
`dotnet ef migrations add ...` + `dotnet ef database update` sinh ra từ các class C#.

Bộ SQL này dựng schema bằng tay, nên có hai cách phối hợp:

1. **Dùng tạm để học và thử API** — khi backend đã có Migration đầu tiên thì chạy
   `docker compose down -v` để xoá sạch, rồi để EF Core tự tạo lại từ đầu.
2. **Dùng làm bản thiết kế đối chiếu** — viết entity C# sao cho Migration sinh ra
   khớp với các bảng ở đây, rồi so lại bằng `dotnet ef migrations script`.

Đừng chạy song song cả hai trên cùng một database: EF Core sẽ không thấy bảng
`__EFMigrationsHistory` và tưởng database còn trống, dẫn tới lỗi "bảng đã tồn tại".
