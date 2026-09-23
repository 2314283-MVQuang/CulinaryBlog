using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Features.Auth.Dtos;
using CulinaryBlog.Domain.Entities;
using CulinaryBlog.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CulinaryBlog.Application.Features.Auth.Commands.GoogleLogin;

/// <summary>
/// FR-AUTH-003. Flow (mục 5.2): verify idToken với Google → tìm user theo email → nếu CHƯA có thì
/// tạo mới (AvatarUrl lấy từ Google, EmailConfirmed = kết quả verify của Google, KHÔNG có mật khẩu
/// — tài khoản chỉ đăng nhập được qua Google), nếu ĐÃ có thì đăng nhập luôn → sinh cặp token giống
/// hệt LoginCommandHandler/RegisterCommandHandler để FE xử lý response thống nhất.
/// </summary>
public class GoogleLoginCommandHandler(
    IGoogleAuthService googleAuthService,
    UserManager<ApplicationUser> userManager,
    IJwtService jwtService,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<GoogleLoginCommand, AuthResponseDto>
{
    private const string DefaultRole = "Author";

    public async Task<AuthResponseDto> Handle(GoogleLoginCommand request, CancellationToken ct)
    {
        var googleUser = await googleAuthService.VerifyIdTokenAsync(request.IdToken, ct);

        var user = await userManager.FindByEmailAsync(googleUser.Email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = await GenerateUniqueUserNameAsync(googleUser.Email),
                Email = googleUser.Email,
                DisplayName = googleUser.Name,
                AvatarUrl = googleUser.AvatarUrl,
                EmailConfirmed = googleUser.EmailVerified,
            };

            // Không truyền password cho CreateAsync — tài khoản tạo qua Google không có mật khẩu nội
            // bộ (SRS FR-AUTH-003: chỉ đăng nhập lại được bằng Google, không dùng /auth/login).
            var createResult = await userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                throw ToValidationException(createResult);
            }

            await userManager.AddToRoleAsync(user, DefaultRole);
        }

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = jwtService.GenerateAccessToken(user, roles);
        var (rawRefreshToken, refreshTokenHash) = jwtService.GenerateRefreshToken();

        await refreshTokenRepository.AddAsync(
            new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
                CreatedByIp = request.IpAddress,
            },
            ct);

        await unitOfWork.SaveChangesAsync(ct);

        return new AuthResponseDto(
            accessToken,
            rawRefreshToken,
            jwtService.AccessTokenLifetimeSeconds,
            new UserProfileDto(user.Id, user.Email!, user.DisplayName, user.AvatarUrl, user.Bio, roles));
    }

    /// <summary>
    /// ASP.NET Identity bắt buộc UserName duy nhất nhưng Google không cấp UserName — tự sinh từ
    /// phần trước "@" của email, thêm số tăng dần nếu trùng (cùng ý tưởng với SlugHelper cho Recipe,
    /// xem RecipeMappingExtensions).
    /// </summary>
    private async Task<string> GenerateUniqueUserNameAsync(string email)
    {
        var baseUserName = email.Split('@')[0];
        var candidate = baseUserName;
        var suffix = 1;

        while (await userManager.FindByNameAsync(candidate) is not null)
        {
            candidate = $"{baseUserName}{++suffix}";
        }

        return candidate;
    }

    private static ValidationException ToValidationException(IdentityResult result)
    {
        var errors = result.Errors
            .GroupBy(e => e.Code, e => e.Description)
            .ToDictionary(g => g.Key, g => g.ToArray());

        return new ValidationException(errors);
    }
}
