namespace QuanLySP.Helpers;

/// <summary>
/// Quản lý ảnh sản phẩm: ảnh được sao chép vào thư mục Images cạnh file exe,
/// CSDL chỉ lưu tên file nên có thể chép cả thư mục chương trình sang máy khác.
/// </summary>
public static class ImageStore
{
    /// <summary>Đường dẫn thư mục chứa ảnh, tự tạo nếu chưa có.</summary>
    public static string Folder
    {
        get
        {
            var folder = Path.Combine(AppContext.BaseDirectory, "Images");
            Directory.CreateDirectory(folder);
            return folder;
        }
    }

    /// <summary>Bộ lọc dùng cho OpenFileDialog.</summary>
    public const string Filter =
        "File ảnh|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp|Tất cả các file|*.*";

    /// <summary>
    /// Sao chép ảnh người dùng chọn vào thư mục Images với tên duy nhất.
    /// Trả về tên file để lưu xuống cột HinhAnh.
    /// </summary>
    public static string Save(string sourcePath)
    {
        var extension = Path.GetExtension(sourcePath);
        var fileName = $"{Path.GetFileNameWithoutExtension(sourcePath)}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
        var destination = Path.Combine(Folder, fileName);

        File.Copy(sourcePath, destination, overwrite: true);

        return fileName;
    }

    /// <summary>Đổi tên file trong CSDL thành đường dẫn đầy đủ, null nếu không có ảnh.</summary>
    public static string? ResolvePath(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return null;
        }

        // Hỗ trợ cả trường hợp CSDL lưu sẵn đường dẫn tuyệt đối.
        if (Path.IsPathRooted(fileName))
        {
            return File.Exists(fileName) ? fileName : null;
        }

        var path = Path.Combine(Folder, fileName);
        return File.Exists(path) ? path : null;
    }

    /// <summary>
    /// Nạp ảnh thành đối tượng Image mà không giữ khoá file
    /// (Image.FromFile sẽ khoá file cho tới khi Dispose).
    /// </summary>
    public static Image? Load(string? fileName)
    {
        var path = ResolvePath(fileName);
        if (path is null)
        {
            return null;
        }

        try
        {
            using var stream = new MemoryStream(File.ReadAllBytes(path));
            return Image.FromStream(stream);
        }
        catch
        {
            return null;
        }
    }
}
