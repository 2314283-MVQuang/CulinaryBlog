using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CulinaryBlog.Application.Common.Helpers;

/// <summary>
/// Sinh slug SEO-friendly từ tiếng Việt có dấu: chữ thường, bỏ dấu, thay khoảng trắng bằng "-"
/// (FR-CAT-003, mục 5.7). Dùng chung cho Category và Recipe.
/// </summary>
public static partial class SlugHelper
{
    public static string GenerateSlug(string input)
    {
        var withoutDiacritics = RemoveDiacritics(input.Trim().ToLowerInvariant());

        // Thay mọi ký tự không phải chữ/số bằng "-", rồi gộp nhiều "-" liên tiếp thành 1.
        var slug = NonAlphaNumericRegex().Replace(withoutDiacritics, "-");
        slug = MultipleDashesRegex().Replace(slug, "-").Trim('-');

        return string.IsNullOrWhiteSpace(slug) ? Guid.NewGuid().ToString("N")[..8] : slug;
    }

    /// <summary>Nếu slug đã tồn tại, thêm hậu tố số (vd: "pho-bo", "pho-bo-2", "pho-bo-3"...).</summary>
    public static string AppendSuffix(string baseSlug, int suffix) => $"{baseSlug}-{suffix}";

    private static string RemoveDiacritics(string text)
    {
        // Xử lý riêng chữ "đ"/"Đ" vì FormD không tách được ký tự này ra thành "d" + dấu gạch.
        text = text.Replace('đ', 'd').Replace('Đ', 'D');

        var normalized = text.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var c in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex NonAlphaNumericRegex();

    [GeneratedRegex(@"-{2,}")]
    private static partial Regex MultipleDashesRegex();
}
