namespace CulinaryBlog.Application.Features.Auth.Dtos;

/// <summary>Trả về sau Register/Login/Refresh thành công (mục 8.1).</summary>
public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    UserProfileDto User);
