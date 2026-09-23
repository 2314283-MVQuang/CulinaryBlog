using System.Security.Claims;
using CulinaryBlog.API.Extensions;
using CulinaryBlog.Application.Features.Auth.Commands.GoogleLogin;
using CulinaryBlog.Application.Features.Auth.Commands.Login;
using CulinaryBlog.Application.Features.Auth.Commands.Logout;
using CulinaryBlog.Application.Features.Auth.Commands.Refresh;
using CulinaryBlog.Application.Features.Auth.Commands.Register;
using CulinaryBlog.Application.Features.Auth.Commands.UpdateProfile;
using CulinaryBlog.Application.Features.Auth.Queries.Me;
using MediatR;

namespace CulinaryBlog.API.Endpoints;

/// <summary>Mục 8.1.</summary>
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Auth");

        group.MapPost("/register", async (RegisterCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created("/api/v1/auth/me", new { data = result });
        });

        group.MapPost("/login", async (LoginRequest request, HttpContext http, ISender sender) =>
        {
            var command = new LoginCommand(request.Email, request.Password, http.Connection.RemoteIpAddress?.ToString());
            var result = await sender.Send(command);
            return result.ToOkResponse();
        });

        // FR-AUTH-003 — Guest gửi idToken lấy từ Google Sign-In JS SDK (FE chịu trách nhiệm hiển thị
        // nút đăng nhập Google và lấy idToken); BE chỉ verify + tạo/đăng nhập user, trả AuthResponseDto
        // giống hệt /login để FE dùng chung 1 luồng lưu token.
        group.MapPost("/google", async (GoogleLoginRequest request, HttpContext http, ISender sender) =>
        {
            var command = new GoogleLoginCommand(request.IdToken, http.Connection.RemoteIpAddress?.ToString());
            var result = await sender.Send(command);
            return result.ToOkResponse();
        });

        group.MapPost("/refresh", async (RefreshRequest request, HttpContext http, ISender sender) =>
        {
            var command = new RefreshTokenCommand(request.RefreshToken, http.Connection.RemoteIpAddress?.ToString());
            var result = await sender.Send(command);
            return result.ToOkResponse();
        });

        group.MapPost("/logout", async (RefreshRequest request, ISender sender) =>
        {
            await sender.Send(new LogoutCommand(request.RefreshToken));
            return Results.NoContent();
        }).RequireAuthorization();

        group.MapGet("/me", async (ClaimsPrincipal user, ISender sender) =>
        {
            var result = await sender.Send(new GetMeQuery(user.GetUserId()));
            return result.ToOkResponse();
        }).RequireAuthorization();

        // FR-AUTH-007 — chỉ sửa được hồ sơ của CHÍNH MÌNH: UserId lấy từ JWT, không nhận từ body.
        // Field nào không gửi (null) thì giữ nguyên; gửi chuỗi rỗng cho AvatarUrl/Bio = xoá giá trị cũ.
        group.MapPatch("/me", async (UpdateProfileRequest request, ClaimsPrincipal user, ISender sender) =>
        {
            var command = new UpdateProfileCommand(
                user.GetUserId(),
                request.DisplayName,
                request.AvatarUrl,
                request.Bio);

            var result = await sender.Send(command);
            return result.ToOkResponse();
        }).RequireAuthorization();
    }

    private record LoginRequest(string Email, string Password);

    private record GoogleLoginRequest(string IdToken);

    private record RefreshRequest(string RefreshToken);

    /// <summary>Body cho PATCH /auth/me — KHÔNG có UserId (chống sửa hồ sơ người khác).</summary>
    private record UpdateProfileRequest(string? DisplayName, string? AvatarUrl, string? Bio);
}
