# Backend — Culinary Blog API

Khung sườn (scaffold) backend theo đúng kiến trúc trong tài liệu đặc tả: **Clean Architecture 4 tầng
+ CQRS/MediatR**, .NET 10 Minimal API, PostgreSQL 16 qua EF Core.

Đây **chưa phải bản đầy đủ** — xem mục "Đã có gì / Chưa có gì" bên dưới trước khi bắt tay code tiếp,
để biết phần nào làm mẫu sẵn (theo đúng pattern) và phần nào cả nhóm cần tự viết thêm.

---

## 1. Cấu trúc thư mục (khớp mục 6.2 tài liệu đặc tả)

```
src/
├── CulinaryBlog.Domain/          # Entities, Enums, Interfaces — KHÔNG phụ thuộc thư viện ngoài
│                                  # (ngoại lệ: ApplicationUser kế thừa IdentityUser, xem .csproj)
├── CulinaryBlog.Application/     # CQRS: Commands/Queries/Handlers/Validators/DTOs
│   └── Features/
│       ├── Auth/                 # Register, Login, Refresh, Logout, Me
│       ├── Categories/           # GetCategories, CreateCategory
│       └── Recipes/              # GetRecipes, GetRecipeBySlug, CreateRecipe
├── CulinaryBlog.Infrastructure/  # EF Core DbContext, Repositories, JwtService, Identity...
└── CulinaryBlog.API/             # Minimal API endpoints, Program.cs, Dockerfile
```

Quy tắc phụ thuộc (Dependency Rule): `API → Infrastructure → Application → Domain`. Domain không
biết gì về 3 tầng còn lại. Mỗi Feature trong Application đi theo khuôn:
`Command/Query.cs` → `...Validator.cs` (FluentValidation) → `...Handler.cs` (MediatR).
**Copy nguyên khuôn của `Features/Recipes/Commands/CreateRecipe/` khi viết thêm use case mới** —
đó là ví dụ đầy đủ nhất (có validate, có xử lý quan hệ cha-con).

## 2. Chạy thử

```bash
# 1. Khởi động PostgreSQL (đã có schema + seed data sẵn — xem db/README.md)
docker compose up -d postgres

# 2. Khai báo JWT secret (KHÔNG BAO GIỜ commit secret vào Git — CONS mục 5.2)
cd src/CulinaryBlog.API
dotnet user-secrets init
dotnet user-secrets set "Jwt:Secret" "day-la-chuoi-bi-mat-it-nhat-32-ky-tu-doi-truoc-khi-deploy"

# 3. Restore + chạy (từ thư mục gốc repo)
cd ../..
dotnet restore
dotnet run --project src/CulinaryBlog.API
```

Mở `http://localhost:5000/scalar` để xem và thử API (Scalar UI, thay cho Swagger UI cổ điển).

> **Môi trường dựng scaffold này không có internet để tra cứu phiên bản NuGet mới nhất** — các
> `Version="..."` trong file `.csproj` là ước lượng tốt nhất tại thời điểm viết. Nếu
> `dotnet restore` báo lỗi version không tồn tại, chạy `dotnet add package <Tên>` (không kèm
> `--version`) để tự lấy bản mới nhất tương thích — sau đó `dotnet build` để chắc mọi thứ compile.
> Đây là bước bắt buộc đầu tiên khi ai đó trong nhóm pull code về.

## 3. Đã có gì (chạy được, đúng pattern đặc tả)

| Module | Đã làm |
|---|---|
| **Kiến trúc** | Đủ 4 tầng, CQRS/MediatR, Pipeline Behavior (Logging + Validation), FluentValidation, EF Core Code-First, RFC 7807 error response, named authorization policy |
| **Auth** | Register, Login (khóa 5 lần sai), Refresh (Token Rotation + Reuse Detection), Logout, Me — JWT HS256 15 phút, PBKDF2 qua Identity |
| **Categories** | GET danh sách (kèm số recipe Published), POST tạo mới (Admin, tự sinh slug) |
| **Recipes** | GET danh sách (phân trang/lọc/sắp xếp), GET chi tiết theo slug, POST tạo mới (Draft, kèm steps/ingredients) |
| **Database** | Entity + EF Core Configuration khớp CHÍNH XÁC với `db/init/02-schema.sql` đã có sẵn |

## 4. Chưa có gì — việc còn lại cho cả nhóm (chia theo FR code trong tài liệu đặc tả)

Mỗi mục dưới đây có `TODO` comment ngay tại vị trí liên quan trong code — tìm bằng
`grep -rn "TODO" src/` để thấy hết.

- **FR-AUTH-003** Google OAuth 2.0 — chưa có endpoint `/auth/google`.
- **FR-AUTH-007** `PATCH /auth/me` cập nhật profile — chưa có.
- **FR-CAT-002/004/005** Chi tiết danh mục kèm recipe, sửa, xóa (kiểm tra `HasRecipesAsync` → 409) — chưa có. `ICategoryRepository.HasRecipesAsync` đã viết sẵn, chỉ cần dùng.
- **FR-RCP-004/005/006/007** Update (cần header `If-Match` + `RecipeAuthorizationHandler` cho Resource-Based Authorization), Publish/Unpublish, Archive, Delete (hard delete) — chưa có.
- **FR-RCP-008/009/010** CRUD ảnh, nguyên liệu, bước riêng lẻ sau khi đã tạo recipe — chưa có (tạo kèm lúc POST /recipes thì đã có).
- **FR-SRCH-001** Full-Text Search (`GET /recipes/search`) — chưa có, cột `SearchVector` + trigger đã có sẵn ở DB (`db/init/02-schema.sql`), chỉ cần viết Query dùng `EF.Functions.ToTsQuery` / raw SQL tham số hóa.
- **Redis** — chưa tích hợp. `CachingBehavior`/`CacheInvalidationBehavior` (mục 6.3) chưa viết, category/recipe đang query thẳng DB mỗi lần.
- **MinIO** — chưa tích hợp. `IFileStorageService` đã có interface chuẩn, bản hiện tại (`LocalFileStorageService`) lưu vào đĩa cục bộ tạm thời. Viết `MinioFileStorageService` implement cùng interface rồi đổi 1 dòng DI.
- **Hangfire** — chưa tích hợp. Email chào mừng hiện gọi đồng bộ (`ConsoleEmailService` chỉ log ra console); resize ảnh (FR-JOB-002) và sitemap (FR-JOB-003) chưa có.
- **Serilog / OpenTelemetry / Rate Limiting / Health checks thật** (FR-OBS-001/002/003, mục 5.2) — chưa có, `/health` hiện chỉ trả cứng `{status: "healthy"}`.
- **Google OAuth, MinIO, Redis, Hangfire, Seq, Mailhog** trong `docker-compose.yml` — chưa thêm service (mục 6.4 liệt kê đủ danh sách).
- **Mã lỗi chi tiết mục 10.2** (vd `RECIPE_CONCURRENCY_CONFLICT`, `CATEGORY_DELETE_HAS_RECIPES`) — `GlobalExceptionMiddleware` hiện dùng message chung, chưa gắn field `type`/mã lỗi cụ thể.
- **Test** — chưa có project test nào (mục 5.5 yêu cầu unit test ≥80% Application layer + integration test mọi endpoint).

## 5. Vài quyết định thiết kế cần biết trước khi sửa code

- **`RowVersion` không bao giờ set tay trong C#.** Trigger `touch_row()` ở PostgreSQL (xem
  `db/init/02-schema.sql`) tự sinh giá trị mới mỗi lần UPDATE — EF Core chỉ đọc lại
  (`IsRowVersion()` trong mỗi `*Configuration.cs`) để dùng làm optimistic concurrency token.
- **Cột `SearchVector` của Recipe không map vào C#** — trigger DB tự quản lý hoàn toàn.
- **`ApplicationUser` (Domain) kế thừa `IdentityUser`** — ngoại lệ duy nhất cho quy tắc "Domain
  không phụ thuộc thư viện ngoài" (xem comment trong `CulinaryBlog.Domain.csproj`), cần thiết vì
  tài liệu đặc tả liệt kê `ApplicationUser` là entity của Domain (mục 6.2/7.7).
- **Không dùng EF Core Migrations song song với `db/init/*.sql`.** Database hiện được tạo bằng
  script SQL viết tay (đã chạy sẵn qua Docker). Nếu chuyển sang `dotnet ef migrations add`, đọc kỹ
  mục cuối `db/README.md` trước — chạy song song cả hai cách sẽ lỗi "bảng đã tồn tại".
- **Response mọi API bọc trong `{ "data": ..., "meta": ... }`** (mục 8) — dùng
  `ApiResponseExtensions.ToOkResponse()` / `.ToPagedResponse()`, không tự viết `Results.Ok(...)` thủ công.
- **Không hardcode role string** — dùng `AuthorizationPolicies.Admin` / `.Author`
  (`API/Extensions/AuthorizationPolicies.cs`), không viết `.RequireAuthorization("Admin")` trực tiếp.
