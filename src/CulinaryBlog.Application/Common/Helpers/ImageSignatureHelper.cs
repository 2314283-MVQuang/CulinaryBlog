namespace CulinaryBlog.Application.Common.Helpers;

/// <summary>
/// FR-RCP-008 bước 4: nhận dạng ảnh bằng "magic bytes" — vài byte đầu của file THẬT — thay vì tin
/// header Content-Type do client khai báo. Header đó client tự đặt được, nên chỉ kiểm Content-Type
/// là đổi một dòng lệnh curl đã đẩy được file .exe lên máy chủ dưới tên .png.
///
/// Trả về MIME type suy ra từ nội dung, hoặc null nếu không khớp định dạng ảnh nào được hỗ trợ.
/// </summary>
public static class ImageSignatureHelper
{
    /// <summary>
    /// Đọc 32 byte đầu là đủ: chữ ký dài nhất là hộp "ftyp" của AVIF, trong đó ngoài major brand
    /// (byte 8–11) còn phải quét danh sách compatible brands nối tiếp phía sau.
    /// </summary>
    private const int HeaderLength = 32;

    private static ReadOnlySpan<byte> PngSignature => [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    /// <summary>
    /// Đọc phần đầu của <paramref name="content"/> rồi TRẢ CON TRỎ VỀ VỊ TRÍ CŨ, để người gọi
    /// tiếp tục upload nguyên vẹn cả file. Stream bắt buộc phải seek được — luồng lấy từ
    /// IFormFile.OpenReadStream() của ASP.NET Core luôn thoả điều kiện này vì form đã được đệm
    /// sẵn vào bộ nhớ hoặc file tạm.
    /// </summary>
    public static async Task<string?> DetectMimeTypeAsync(Stream content, CancellationToken ct = default)
    {
        if (!content.CanSeek)
        {
            throw new ArgumentException(
                "Cần stream seek được để kiểm tra magic bytes mà không làm hỏng dữ liệu upload.",
                nameof(content));
        }

        var position = content.Position;

        try
        {
            var header = new byte[HeaderLength];
            var read = await content.ReadAtLeastAsync(header, HeaderLength, throwOnEndOfStream: false, ct);
            return Detect(header.AsSpan(0, read));
        }
        finally
        {
            content.Position = position;
        }
    }

    private static string? Detect(ReadOnlySpan<byte> header)
    {
        // JPEG: FF D8 FF (SOI marker + byte đầu của marker kế tiếp).
        if (header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
        {
            return "image/jpeg";
        }

        if (header.Length >= PngSignature.Length && header[..PngSignature.Length].SequenceEqual(PngSignature))
        {
            return "image/png";
        }

        // WebP là container RIFF: "RIFF" ở byte 0–3, 4 byte kích thước, rồi "WEBP" ở byte 8–11.
        if (header.Length >= 12
            && header[..4].SequenceEqual("RIFF"u8)
            && header[8..12].SequenceEqual("WEBP"u8))
        {
            return "image/webp";
        }

        // AVIF là hộp ISO-BMFF: 4 byte kích thước, "ftyp", major brand, 4 byte minor version,
        // rồi danh sách compatible brands.
        if (header.Length >= 12 && header[4..8].SequenceEqual("ftyp"u8) && HasAvifBrand(header))
        {
            return "image/avif";
        }

        return null;
    }

    /// <summary>
    /// Quét major brand lẫn compatible brands. Phải quét cả hai vì nhiều bộ mã hoá ghi major brand
    /// là "mif1" (dùng chung với HEIC) rồi mới liệt kê "avif" ở compatible brands — chỉ đọc major
    /// brand sẽ loại oan các file AVIF hợp lệ. Đổi lại, KHÔNG chấp nhận "mif1" đứng một mình để
    /// tránh nhận nhầm HEIC (định dạng không nằm trong danh sách cho phép của FR-RCP-008).
    /// </summary>
    private static bool HasAvifBrand(ReadOnlySpan<byte> header)
    {
        for (var offset = 8; offset + 4 <= header.Length; offset += 4)
        {
            // Bỏ qua byte 12–15: đó là minor version (một con số), không phải brand.
            if (offset == 12)
            {
                continue;
            }

            var brand = header[offset..(offset + 4)];
            if (brand.SequenceEqual("avif"u8) || brand.SequenceEqual("avis"u8))
            {
                return true;
            }
        }

        return false;
    }
}
