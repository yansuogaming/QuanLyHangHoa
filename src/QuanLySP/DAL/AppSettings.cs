using System.Text.Json;

namespace QuanLySP.DAL;

/// <summary>
/// Đọc file appsettings.json nằm cạnh file exe.
/// Dùng System.Text.Json để không phải thêm package cấu hình.
/// </summary>
public static class AppSettings
{
    private static readonly JsonElement Root = Load();

    /// <summary>Chuỗi kết nối người dùng tự khai báo (có thể rỗng).</summary>
    public static string ConnectionString =>
        GetString("ConnectionStrings", "QuanLyHangHoa") ?? string.Empty;

    /// <summary>Auto | SqlServer | Sqlite. Mặc định Auto.</summary>
    public static string Provider =>
        GetString("Database", "Provider") is { Length: > 0 } value ? value : "Auto";

    /// <summary>Tên CSDL, mặc định QuanLyHangHoa.</summary>
    public static string DatabaseName =>
        GetString("Database", "Name") ?? "QuanLyHangHoa";

    /// <summary>Có tự tạo CSDL + dữ liệu mẫu khi chưa tồn tại hay không.</summary>
    public static bool AutoCreate
    {
        get
        {
            if (Root.ValueKind == JsonValueKind.Object
                && Root.TryGetProperty("Database", out var db)
                && db.TryGetProperty("AutoCreate", out var value)
                && value.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                return value.GetBoolean();
            }

            return true;
        }
    }

    private static JsonElement Load()
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(path))
            {
                return default;
            }

            using var doc = JsonDocument.Parse(File.ReadAllText(path));
            return doc.RootElement.Clone();
        }
        catch
        {
            // File cấu hình hỏng thì dùng giá trị mặc định thay vì làm sập ứng dụng.
            return default;
        }
    }

    private static string? GetString(string section, string key)
    {
        if (Root.ValueKind == JsonValueKind.Object
            && Root.TryGetProperty(section, out var node)
            && node.TryGetProperty(key, out var value)
            && value.ValueKind == JsonValueKind.String)
        {
            return value.GetString();
        }

        return null;
    }
}
