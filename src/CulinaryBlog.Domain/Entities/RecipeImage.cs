using CulinaryBlog.Domain.Common;

namespace CulinaryBlog.Domain.Entities;

/// <summary>Một ảnh của công thức (bảng "RecipeImages" — mục 7.5, FR-RCP-008).</summary>
public class RecipeImage : BaseEntity
{
    public Guid RecipeId { get; set; }

    public Recipe Recipe { get; set; } = null!;

    /// <summary>".../recipes/{recipeId}/{guid}.jpg" — TODO: hiện lưu qua IFileStorageService
    /// (bản local filesystem tạm thời), sau này đổi sang MinIO (FR-FILE-001).</summary>
    public string OriginalUrl { get; set; } = null!;

    /// <summary>800×600 — sinh bởi background job resize ảnh (FR-JOB-002, chưa triển khai — TODO Hangfire).</summary>
    public string? MediumUrl { get; set; }

    /// <summary>300×300 — sinh bởi background job resize ảnh (FR-JOB-002, chưa triển khai — TODO Hangfire).</summary>
    public string? ThumbnailUrl { get; set; }

    public string? AltText { get; set; }

    /// <summary>Chỉ 1 ảnh IsPrimary=true cho mỗi Recipe — ràng buộc bằng partial unique index ở DB.</summary>
    public bool IsPrimary { get; set; }

    public int OrderIndex { get; set; }
}
