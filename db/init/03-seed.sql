-- =============================================================================
--  03 — DỮ LIỆU MẪU
--
--  Mục đích: có sẵn danh mục, 2 tài khoản và vài công thức để test API/giao diện
--  ngay sau khi dựng database, không phải ngồi nhập tay.
--
--  Tất cả lệnh đều dùng ON CONFLICT DO NOTHING nên chạy lại nhiều lần cũng an toàn.
-- =============================================================================


-- -----------------------------------------------------------------------------
-- ROLES (mục 2)
-- "Guest" không phải role trong DB — đó là người dùng chưa đăng nhập.
-- -----------------------------------------------------------------------------
INSERT INTO "AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp") VALUES
    ('24b35d9c-bf22-483b-bc9e-8a3ead457eb3', 'Admin',  'ADMIN',  gen_random_uuid()::text),
    ('1e705edd-1c3d-44af-b4af-4494ce2811fd', 'Author', 'AUTHOR', gen_random_uuid()::text)
ON CONFLICT ("Id") DO NOTHING;


-- -----------------------------------------------------------------------------
-- TÀI KHOẢN MẪU
--
--   admin@culinaryblog.local  / Admin@123    (role Admin)
--   author@culinaryblog.local / Author@123   (role Author)
--
-- PasswordHash bên dưới đúng định dạng PBKDF2 của ASP.NET Core Identity
-- (v3: HMAC-SHA256, 100.000 vòng lặp) nên đăng nhập được ngay qua API.
--
-- ĐÂY LÀ MẬT KHẨU DÙNG ĐỂ HỌC/TEST TRÊN MÁY CÁ NHÂN.
-- Trước khi đưa lên máy chủ thật, XOÁ hai tài khoản này hoặc đổi mật khẩu.
-- -----------------------------------------------------------------------------
INSERT INTO "AspNetUsers" (
    "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed",
    "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
    "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount",
    "DisplayName", "Bio", "IsActive"
) VALUES
(
    'da2830f1-989a-49f9-8020-5c9ddcfcabd3',
    'admin', 'ADMIN', 'admin@culinaryblog.local', 'ADMIN@CULINARYBLOG.LOCAL', true,
    'AQAAAAEAAYagAAAAEHGL7j5gkhtdcPVmukMPnCq4Snx69XcGXIo4N+kyRXD7eX3aX+IPnE7pgL9uXEWOhA==',
    gen_random_uuid()::text, gen_random_uuid()::text,
    false, false, true, 0,
    'Quản trị viên', 'Tài khoản quản trị dùng để thử nghiệm.', true
),
(
    '36e114f2-bf65-455c-a295-935237cc7e14',
    'bepnha', 'BEPNHA', 'author@culinaryblog.local', 'AUTHOR@CULINARYBLOG.LOCAL', true,
    'AQAAAAEAAYagAAAAEGPqenitzENRCgJ298PtuT1IJhZ9rpAbjJt5sz8Js7eOO4Fv29AyJWJaPu4hI+q1dQ==',
    gen_random_uuid()::text, gen_random_uuid()::text,
    false, false, true, 0,
    'Bếp Nhà', 'Nấu ăn mỗi ngày, chia sẻ món ngon dễ làm cho cả gia đình.', true
)
ON CONFLICT ("Id") DO NOTHING;


INSERT INTO "AspNetUserRoles" ("UserId", "RoleId") VALUES
    -- Admin được gán CẢ Admin lẫn Author: theo mục 2, Admin có mọi quyền của Author.
    ('da2830f1-989a-49f9-8020-5c9ddcfcabd3', '24b35d9c-bf22-483b-bc9e-8a3ead457eb3'),
    ('da2830f1-989a-49f9-8020-5c9ddcfcabd3', '1e705edd-1c3d-44af-b4af-4494ce2811fd'),
    ('36e114f2-bf65-455c-a295-935237cc7e14', '1e705edd-1c3d-44af-b4af-4494ce2811fd')
ON CONFLICT ("UserId", "RoleId") DO NOTHING;


-- -----------------------------------------------------------------------------
-- DANH MỤC (mục 7.6)
-- -----------------------------------------------------------------------------
INSERT INTO "Categories" ("Name", "Slug", "Description", "OrderIndex") VALUES
    ('Món khai vị',   'mon-khai-vi',  'Món ăn nhẹ mở đầu bữa ăn: gỏi, cuốn, salad.',          1),
    ('Món chính',     'mon-chinh',    'Món mặn ăn cùng cơm, bún, phở — trọng tâm bữa ăn.',     2),
    ('Canh & Súp',    'canh-sup',     'Các loại canh, súp, lẩu nóng hổi.',                     3),
    ('Món nướng',     'mon-nuong',    'Nướng than, nướng lò, nướng nồi chiên không dầu.',      4),
    ('Món chay',      'mon-chay',     'Món không dùng thịt cá, phù hợp ngày rằm và ăn nhẹ.',   5),
    ('Tráng miệng',   'trang-mieng',  'Chè, bánh ngọt, hoa quả dầm.',                          6),
    ('Đồ uống',       'do-uong',      'Nước ép, sinh tố, trà, cà phê pha tại nhà.',            7),
    ('Bữa sáng',      'bua-sang',     'Món nhanh gọn cho buổi sáng bận rộn.',                  8)
ON CONFLICT ("Slug") DO NOTHING;


-- -----------------------------------------------------------------------------
-- CÔNG THỨC MẪU (mục 7.2) — cả 3 đều ở trạng thái Published (Status = 1)
--
-- Không seed bảng "RecipeImages" vì ảnh phải nằm trên MinIO, mà MinIO thì chưa
-- chạy. Giao diện đã có sẵn ảnh thay thế (nền cam + emoji) cho công thức chưa có
-- ảnh nên vẫn hiển thị đẹp.
-- -----------------------------------------------------------------------------
INSERT INTO "Recipes" (
    "Id", "Title", "Slug", "Description", "Instructions",
    "PrepTime", "CookTime", "Servings", "Difficulty", "Status",
    "CategoryId", "AuthorId", "PublishedAt",
    "Nutrition_Calories", "Nutrition_Protein", "Nutrition_Carbohydrates",
    "Nutrition_Fat", "Nutrition_Fiber", "Nutrition_Sodium"
) VALUES
(
    'a1111111-1111-4111-8111-111111111111',
    'Phở bò Hà Nội',
    'pho-bo-ha-noi',
    'Nước dùng trong, ngọt xương, thơm mùi quế hồi — công thức phở bò chuẩn vị Hà Nội nấu tại nhà.',
    E'Bí quyết nằm ở nước dùng: xương phải chần kỹ rồi ninh lửa nhỏ, hớt bọt liên tục thì nước mới trong.\n\nGia vị nướng thơm trước khi thả vào nồi sẽ dậy mùi hơn nhiều so với thả sống.',
    40, 180, 4, 3, 1,
    (SELECT "Id" FROM "Categories" WHERE "Slug" = 'mon-chinh'),
    '36e114f2-bf65-455c-a295-935237cc7e14',
    now() - interval '5 days',
    520.00, 32.50, 62.00, 14.00, 3.20, 980.00
),
(
    'a2222222-2222-4222-8222-222222222222',
    'Gỏi cuốn tôm thịt',
    'goi-cuon-tom-thit',
    'Món cuốn thanh mát, ít dầu mỡ, chấm cùng tương đậu phộng béo bùi. Không cần bếp núc cầu kỳ.',
    E'Bánh tráng chỉ nhúng nước thật nhanh rồi để ráo vài giây, đừng ngâm lâu kẻo bị nhão và rách khi cuốn.',
    30, 15, 4, 1, 1,
    (SELECT "Id" FROM "Categories" WHERE "Slug" = 'mon-khai-vi'),
    '36e114f2-bf65-455c-a295-935237cc7e14',
    now() - interval '3 days',
    210.00, 14.00, 26.00, 5.50, 2.80, 430.00
),
(
    'a3333333-3333-4333-8333-333333333333',
    'Chè đậu xanh nước cốt dừa',
    'che-dau-xanh-nuoc-cot-dua',
    'Chè đậu xanh mềm bở, thơm béo nước cốt dừa — món tráng miệng giải nhiệt quen thuộc.',
    E'Ngâm đậu trước vài tiếng thì nấu nhanh nhừ hơn hẳn. Cho đường sau khi đậu đã mềm, cho sớm đậu sẽ sượng.',
    15, 30, 4, 1, 1,
    (SELECT "Id" FROM "Categories" WHERE "Slug" = 'trang-mieng'),
    'da2830f1-989a-49f9-8020-5c9ddcfcabd3',
    now() - interval '1 day',
    280.00, 8.00, 48.00, 7.00, 4.10, 60.00
)
ON CONFLICT ("Id") DO NOTHING;


-- -----------------------------------------------------------------------------
-- NGUYÊN LIỆU (mục 7.4)
-- -----------------------------------------------------------------------------
INSERT INTO "RecipeIngredients" ("RecipeId", "Name", "Quantity", "Unit", "Notes", "OrderIndex") VALUES
    -- Phở bò
    ('a1111111-1111-4111-8111-111111111111', 'Xương ống bò',      1.500, 'kg',    'Chặt khúc, chần qua nước sôi', 0),
    ('a1111111-1111-4111-8111-111111111111', 'Thịt bắp bò',       0.500, 'kg',    NULL,                            1),
    ('a1111111-1111-4111-8111-111111111111', 'Bánh phở tươi',     0.800, 'kg',    NULL,                            2),
    ('a1111111-1111-4111-8111-111111111111', 'Hành tây',          1.000, 'củ',    'Nướng sém vỏ',                  3),
    ('a1111111-1111-4111-8111-111111111111', 'Gừng',              1.000, 'nhánh', 'Nướng thơm, đập dập',           4),
    ('a1111111-1111-4111-8111-111111111111', 'Quế, hồi, thảo quả',1.000, 'phần',  'Rang thơm, cho vào túi vải',    5),
    ('a1111111-1111-4111-8111-111111111111', 'Nước mắm',         60.000, 'ml',    NULL,                            6),
    -- Gỏi cuốn
    ('a2222222-2222-4222-8222-222222222222', 'Bánh tráng',       12.000, 'cái',   NULL,                            0),
    ('a2222222-2222-4222-8222-222222222222', 'Tôm sú',            0.300, 'kg',    'Luộc, bóc vỏ, chẻ đôi',         1),
    ('a2222222-2222-4222-8222-222222222222', 'Thịt ba chỉ',       0.250, 'kg',    'Luộc, thái lát mỏng',           2),
    ('a2222222-2222-4222-8222-222222222222', 'Bún tươi',          0.200, 'kg',    NULL,                            3),
    ('a2222222-2222-4222-8222-222222222222', 'Rau sống các loại', 1.000, 'phần',  'Xà lách, hẹ, rau thơm',         4),
    ('a2222222-2222-4222-8222-222222222222', 'Tương đậu phộng',   150.000,'ml',   NULL,                            5),
    -- Chè đậu xanh
    ('a3333333-3333-4333-8333-333333333333', 'Đậu xanh cà vỏ',    0.250, 'kg',    'Ngâm nước 3-4 tiếng',           0),
    ('a3333333-3333-4333-8333-333333333333', 'Đường cát',         0.150, 'kg',    'Nêm theo khẩu vị',              1),
    ('a3333333-3333-4333-8333-333333333333', 'Nước cốt dừa',      200.000,'ml',   NULL,                            2),
    ('a3333333-3333-4333-8333-333333333333', 'Bột năng',         20.000, 'g',     'Hoà nước lạnh để tạo độ sánh',  3),
    ('a3333333-3333-4333-8333-333333333333', 'Lá dứa',            2.000, 'lá',    'Buộc gọn, cho vào khi nấu',     4)
ON CONFLICT DO NOTHING;


-- -----------------------------------------------------------------------------
-- CÁC BƯỚC THỰC HIỆN (mục 7.3)
-- -----------------------------------------------------------------------------
INSERT INTO "RecipeSteps" ("RecipeId", "StepNumber", "Title", "Description", "TimerMinutes") VALUES
    -- Phở bò
    ('a1111111-1111-4111-8111-111111111111', 1, 'Sơ chế xương',
     'Rửa sạch xương ống, chần qua nước sôi 3 phút rồi rửa lại cho hết bọt bẩn. Bước này quyết định nước dùng có trong hay không.', 10),
    ('a1111111-1111-4111-8111-111111111111', 2, 'Nướng gia vị',
     'Nướng hành tây và gừng trên bếp đến khi sém vỏ, thơm. Rang quế hồi thảo quả trên chảo khô khoảng 2 phút.', 15),
    ('a1111111-1111-4111-8111-111111111111', 3, 'Ninh nước dùng',
     'Cho xương vào nồi cùng 4 lít nước, đun sôi rồi hạ lửa nhỏ liu riu. Thả túi gia vị, hành gừng đã nướng. Hớt bọt thường xuyên trong suốt quá trình ninh.', 150),
    ('a1111111-1111-4111-8111-111111111111', 4, 'Nêm nếm và hoàn thiện',
     'Nêm nước mắm, muối, chút đường phèn cho vừa miệng. Trụng bánh phở, xếp thịt bò thái mỏng lên trên rồi chan nước dùng đang sôi.', 15),
    -- Gỏi cuốn
    ('a2222222-2222-4222-8222-222222222222', 1, 'Luộc tôm và thịt',
     'Luộc tôm với chút muối đến khi vừa chín tới, vớt ra ngâm nước đá cho giòn. Thịt ba chỉ luộc chín, để nguội rồi thái lát mỏng.', 15),
    ('a2222222-2222-4222-8222-222222222222', 2, 'Chuẩn bị rau và bún',
     'Rửa sạch rau sống, để thật ráo nước. Bún tươi xả qua nước sôi rồi để ráo.', 10),
    ('a2222222-2222-4222-8222-222222222222', 3, 'Cuốn',
     'Nhúng bánh tráng qua nước rồi trải ra. Xếp lần lượt rau, bún, thịt, tôm rồi cuốn chặt tay. Đặt tôm ở ngoài cùng cho đẹp mắt.', 15),
    ('a2222222-2222-4222-8222-222222222222', 4, 'Pha nước chấm',
     'Đun nóng tương đậu phộng với chút nước, thêm đường và tỏi phi. Rắc đậu phộng rang giã dập lên trên.', 5),
    -- Chè đậu xanh
    ('a3333333-3333-4333-8333-333333333333', 1, 'Ngâm và nấu đậu',
     'Đậu xanh ngâm 3-4 tiếng cho nở, đãi sạch. Cho vào nồi cùng 1 lít nước và lá dứa, nấu lửa vừa đến khi đậu mềm bở.', 30),
    ('a3333333-3333-4333-8333-333333333333', 2, 'Thêm đường',
     'Khi đậu đã mềm hẳn mới cho đường vào, khuấy đều và nấu thêm 5 phút. Cho đường quá sớm đậu sẽ bị sượng.', 5),
    ('a3333333-3333-4333-8333-333333333333', 3, 'Tạo độ sánh',
     'Hoà bột năng với nước lạnh rồi từ từ rưới vào nồi chè, vừa rưới vừa khuấy đến khi đạt độ sánh mong muốn.', 5),
    ('a3333333-3333-4333-8333-333333333333', 4, 'Thắng nước cốt dừa',
     'Đun nước cốt dừa với chút muối và bột năng cho sánh nhẹ. Múc chè ra chén rồi rưới nước cốt dừa lên trên.', 5)
ON CONFLICT ("RecipeId", "StepNumber") DO NOTHING;
