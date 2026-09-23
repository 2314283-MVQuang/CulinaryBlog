using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CulinaryBlog.Infrastructure.Persistence.Seed;

/// <summary>
/// Đảm bảo các role hệ thống (Admin, Author) tồn tại trong bảng AspNetRoles.
///
/// Lý do cần seeder riêng: RegisterCommandHandler gọi
/// <c>UserManager.AddToRoleAsync(user, "Author")</c> ngay sau khi tạo tài khoản mới. Nếu role
/// "Author" chưa tồn tại, thao tác đó NÉM <see cref="InvalidOperationException"/> thẳng ra ngoài
/// (không trả về IdentityResult thất bại nhẹ nhàng như CreateAsync) — làm /auth/register trả về
/// lỗi 500 thay vì tạo được tài khoản.
///
/// Khác với <see cref="DbSeeder"/> (chỉ sinh dữ liệu MẪU ở Development), seeder này PHẢI chạy ở
/// MỌI môi trường kể cả production — vì đây là dữ liệu hệ thống bắt buộc để tính năng đăng ký
/// hoạt động, không phải dữ liệu demo. Xem lời gọi ở Program.cs (đứng NGOÀI khối
/// <c>if (app.Environment.IsDevelopment())</c>).
///
/// Idempotent: chỉ tạo role nào CHƯA có (kiểm tra bằng RoleExistsAsync trước), an toàn khi gọi
/// lại ở mỗi lần khởi động app.
/// </summary>
public static class RoleSeeder
{
    /// <summary>Toàn bộ role hệ thống đang dùng trong project (mục 2.1, 5.2: Admin/Author).</summary>
    public static readonly string[] SystemRoles = ["Admin", "Author"];

    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, ILogger? logger = null)
    {
        foreach (var roleName in SystemRoles)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                logger?.LogInformation("[RoleSeeder] Đã tạo role '{Role}'.", roleName);
            }
            else
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                logger?.LogError("[RoleSeeder] Tạo role '{Role}' thất bại: {Errors}", roleName, errors);
            }
        }
    }
}
