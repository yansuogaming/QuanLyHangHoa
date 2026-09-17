using System.Reflection;

namespace QuanLySP.Helpers;

/// <summary>
/// Thông tin nhận diện phần mềm, đọc từ metadata của assembly
/// (khai báo trong QuanLySP.csproj) để chỉ phải sửa ở một nơi.
/// </summary>
public static class ThongTinUngDung
{
    private static readonly Assembly Asm = Assembly.GetExecutingAssembly();

    /// <summary>Tên sản phẩm, ví dụ "Quản Lý Sản Phẩm".</summary>
    public static string Ten =>
        Asm.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "Quản Lý Sản Phẩm";

    /// <summary>Số phiên bản rút gọn, ví dụ "1.0".</summary>
    public static string PhienBan
    {
        get
        {
            var v = Asm.GetName().Version;
            return v is null ? "1.0" : $"{v.Major}.{v.Minor}";
        }
    }

    /// <summary>Tác giả build, lấy từ thuộc tính Company.</summary>
    public static string TacGia =>
        Asm.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "Yansuo";

    /// <summary>Dòng bản quyền.</summary>
    public static string BanQuyen =>
        Asm.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? string.Empty;

    /// <summary>Tiêu đề dùng cho thanh tiêu đề cửa sổ.</summary>
    public static string TieuDeCuaSo => $"{Ten}  v{PhienBan}";

    /// <summary>Dòng ghi công hiển thị trên giao diện.</summary>
    public static string DongTacGia => $"Phiên bản {PhienBan}  •  Build: {TacGia}";

    /// <summary>
    /// Icon của phần mềm, nạp từ tài nguyên nhúng.
    /// Trả về null nếu vì lý do nào đó không nạp được (không làm sập ứng dụng).
    /// </summary>
    public static Icon? NapIcon()
    {
        try
        {
            using var stream = Asm.GetManifestResourceStream("QuanLySP.app.ico");
            return stream is null ? null : new Icon(stream);
        }
        catch
        {
            return null;
        }
    }
}
