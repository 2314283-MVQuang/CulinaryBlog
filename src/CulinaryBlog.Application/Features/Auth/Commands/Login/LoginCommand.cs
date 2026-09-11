using CulinaryBlog.Application.Features.Auth.Dtos;
using MediatR;

namespace CulinaryBlog.Application.Features.Auth.Commands.Login;

/// <summary>FR-AUTH-002 — POST /api/v1/auth/login (Actor: Author/Admin).</summary>
public record LoginCommand(string Email, string Password, string? IpAddress = null) : IRequest<AuthResponseDto>;
