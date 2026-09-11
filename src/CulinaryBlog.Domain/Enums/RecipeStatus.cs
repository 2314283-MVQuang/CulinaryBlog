namespace CulinaryBlog.Domain.Enums;

/// <summary>Trạng thái công thức (mục 7.2). Lưu trong DB dưới dạng smallint 0-2.</summary>
public enum RecipeStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2,
}
