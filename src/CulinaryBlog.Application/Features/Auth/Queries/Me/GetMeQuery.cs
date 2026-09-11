using CulinaryBlog.Application.Features.Auth.Dtos;
using MediatR;

namespace CulinaryBlog.Application.Features.Auth.Queries.Me;

/// <summary>FR-AUTH-006 — GET /api/v1/auth/me. UserId lấy từ ICurrentUser (claims JWT), không truyền qua body.</summary>
public record GetMeQuery(string UserId) : IRequest<UserProfileDto>;
