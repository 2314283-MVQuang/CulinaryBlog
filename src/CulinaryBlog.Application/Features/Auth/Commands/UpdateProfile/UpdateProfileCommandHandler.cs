using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Features.Auth.Dtos;
using CulinaryBlog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CulinaryBlog.Application.Features.Auth.Commands.UpdateProfile;

/// <summary>
/// FR-AUTH-007. Chỉ 3 field DisplayName/AvatarUrl/Bio được phép đổi — Email, PasswordHash,
/// Role, IsActive KHÔNG bao giờ sửa qua endpoint này (đổi email/mật khẩu là luồng riêng có
/// xác thực lại, đổi role là quyền của Admin).
///
/// Không cần IUnitOfWork: UserManager.UpdateAsync() tự ghi xuống DB qua Identity store,
/// giống cách RegisterCommandHandler dùng UserManager.CreateAsync().
/// </summary>
public class UpdateProfileCommandHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<UpdateProfileCommand, UserProfileDto>
{
    public async Task<UserProfileDto> Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new NotFoundException(nameof(ApplicationUser), request.UserId);

        // Quy ước PATCH: null = client không gửi field này = giữ nguyên.
        // Chuỗi rỗng = client muốn XOÁ giá trị cũ (chỉ áp dụng cho AvatarUrl/Bio vì hai cột này nullable).
        if (request.DisplayName is not null)
        {
            user.DisplayName = request.DisplayName.Trim();
        }

        if (request.AvatarUrl is not null)
        {
            user.AvatarUrl = NullIfBlank(request.AvatarUrl);
        }

        if (request.Bio is not null)
        {
            user.Bio = NullIfBlank(request.Bio);
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw ToValidationException(result);
        }

        var roles = await userManager.GetRolesAsync(user);

        return new UserProfileDto(user.Id, user.Email!, user.DisplayName, user.AvatarUrl, user.Bio, roles);
    }

    private static string? NullIfBlank(string value)
    {
        var trimmed = value.Trim();
        return trimmed.Length == 0 ? null : trimmed;
    }

    /// <summary>Giống RegisterCommandHandler: gom lỗi Identity thành 422 theo RFC 7807.</summary>
    private static ValidationException ToValidationException(IdentityResult result)
    {
        var errors = result.Errors
            .GroupBy(e => e.Code, e => e.Description)
            .ToDictionary(g => g.Key, g => g.ToArray());

        return new ValidationException(errors);
    }
}
