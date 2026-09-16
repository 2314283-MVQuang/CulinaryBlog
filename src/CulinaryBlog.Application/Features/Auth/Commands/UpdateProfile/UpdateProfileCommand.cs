using CulinaryBlog.Application.Features.Auth.Dtos;
using MediatR;

namespace CulinaryBlog.Application.Features.Auth.Commands.UpdateProfile;

/// <summary>
/// FR-AUTH-007 — PATCH /api/v1/auth/me (Actor: Author/Admin đã đăng nhập).
///
/// Đây là PATCH chứ không phải PUT: client chỉ gửi những field muốn đổi, field nào là null
/// thì giữ nguyên giá trị cũ. Muốn xoá Bio/AvatarUrl thì gửi chuỗi rỗng "" (xem Handler).
///
/// UserId KHÔNG nhận từ client — luôn lấy từ JWT ở Endpoint, nếu không ai cũng sửa được hồ sơ
/// của người khác.
/// </summary>
public record UpdateProfileCommand(
    string UserId,
    string? DisplayName,
    string? AvatarUrl,
    string? Bio) : IRequest<UserProfileDto>;
