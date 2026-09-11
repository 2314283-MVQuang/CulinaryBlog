using CulinaryBlog.Application.Features.Auth.Dtos;
using MediatR;

namespace CulinaryBlog.Application.Features.Auth.Commands.Refresh;

/// <summary>FR-AUTH-004 — POST /api/v1/auth/refresh.</summary>
public record RefreshTokenCommand(string RefreshToken, string? IpAddress = null) : IRequest<AuthResponseDto>;
