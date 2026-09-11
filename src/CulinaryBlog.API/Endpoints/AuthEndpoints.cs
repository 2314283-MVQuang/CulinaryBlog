using System.Security.Claims;
using CulinaryBlog.API.Extensions;
using CulinaryBlog.Application.Features.Auth.Commands.Login;
using CulinaryBlog.Application.Features.Auth.Commands.Logout;
using CulinaryBlog.Application.Features.Auth.Commands.Refresh;
using CulinaryBlog.Application.Features.Auth.Commands.Register;
using CulinaryBlog.Application.Features.Auth.Queries.Me;
using MediatR;

namespace CulinaryBlog.API.Endpoints;

/// <summary>Mục 8.1. TODO (nhóm làm tiếp): /auth/google (FR-AUTH-003) và PATCH /auth/me (FR-AUTH-007) chưa triển khai.</summary>
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
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub")!;
            var result = await sender.Send(new GetMeQuery(userId));
            return result.ToOkResponse();
        }).RequireAuthorization();
    }

    private record LoginRequest(string Email, string Password);

    private record RefreshRequest(string RefreshToken);
}
