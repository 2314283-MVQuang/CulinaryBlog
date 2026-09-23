using CulinaryBlog.Application.Common.Exceptions;
using CulinaryBlog.Application.Common.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace CulinaryBlog.Infrastructure.Services;

/// <summary>
/// Implementation IGoogleAuthService bằng gói chính thức Google.Apis.Auth (FR-AUTH-003, mục 5.2).
/// GoogleJsonWebSignature.ValidateAsync tự kiểm tra chữ ký (lấy public key từ Google), hạn dùng, và
/// audience (phải khớp Google:ClientId) — không cần Client Secret cho luồng verify idToken này.
/// </summary>
public class GoogleAuthService(IOptions<GoogleAuthOptions> options) : IGoogleAuthService
{
    private readonly GoogleAuthOptions _options = options.Value;

    public async Task<GoogleUserInfo> VerifyIdTokenAsync(string idToken, CancellationToken ct = default)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_options.ClientId],
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new GoogleUserInfo(payload.Email, payload.Name, payload.Picture, payload.EmailVerified);
        }
        catch (InvalidJwtException)
        {
            // Token sai chữ ký, hết hạn, hoặc sai audience — mục 10.2: AUTH_GOOGLE_TOKEN_INVALID (400).
            throw new BadRequestException("Google idToken không hợp lệ hoặc đã hết hạn.");
        }
    }
}
