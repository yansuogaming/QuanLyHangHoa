using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace QuanLySP.DAL;

/// <summary>Hệ quản trị CSDL đang được sử dụng.</summary>
public enum LoaiCsdl
{
    /// <summary>Microsoft SQL Server (bản chính thức của bài tập).</summary>
    SqlServer,

    /// <summary>SQLite - CSDL dạng một file, dùng khi máy chưa cài SQL Server.</summary>
    Sqlite
}

/// <summary>
/// Quản lý kết nối CSDL cho toàn ứng dụng.
/// <para>
/// Thứ tự ưu tiên khi khởi động (Provider = Auto):
/// 1) chuỗi kết nối khai báo sẵn trong appsettings.json →
/// 2) dò các instance SQL Server thông dụng trên máy →
/// 3) nếu không có SQL Server nào thì tự chuyển sang SQLite (file QuanLyHangHoa.db).
/// </para>
/// </summary>
public static class Db
{
    /// <summary>Các instance SQL Server sẽ thử lần lượt khi không có cấu hình sẵn.</summary>
    private static readonly string[] CandidateServers =
    {
        @"(localdb)\MSSQLLocalDB",
        @".\SQLEXPRESS",
        ".",
        "localhost",
        @".\MSSQLSERVER01",
        @".\SQLEXPRESS01"
    };

    /// <summary>Số giây chờ tối đa cho mỗi lần thử kết nối một instance.</summary>
    private const int ThoiGianChoMoiInstance = 3;

    /// <summary>Số mili-giây chờ tối đa cho toàn bộ quá trình dò (các phép thử chạy song song).</summary>
    private const int TongThoiGianDoToiDa = 12_000;

    /// <summary>Hệ quản trị đang dùng.</summary>
    public static LoaiCsdl Loai { get; private set; } = LoaiCsdl.SqlServer;

    /// <summary>Chuỗi kết nối đang dùng.</summary>
    public static string ConnectionString { get; private set; } = string.Empty;

    /// <summary>Mô tả ngắn để hiển thị trên thanh trạng thái.</summary>
    public static string MoTaKetNoi { get; private set; } = "(chưa kết nối)";

    /// <summary>True khi đã phải lùi về SQLite vì không tìm thấy SQL Server.</summary>
    public static bool DaDungDuPhong { get; private set; }

    /// <summary>Nhật ký các instance SQL Server đã thử và lý do thất bại.</summary>
    public static IReadOnlyList<string> NhatKyDoTimServer => _nhatKy;

    private static readonly List<string> _nhatKy = new();

    /// <summary>Đường dẫn file CSDL SQLite (nằm cạnh file exe).</summary>
    public static string DuongDanFileSqlite =>
        Path.Combine(AppContext.BaseDirectory, $"{AppSettings.DatabaseName}.db");

    /// <summary>
    /// Thiết lập kết nối và bảo đảm CSDL đã sẵn sàng.
    /// Ném <see cref="InvalidOperationException"/> kèm thông báo tiếng Việt nếu thất bại.
    /// </summary>
    public static void Initialize()
    {
        _nhatKy.Clear();

        var provider = AppSettings.Provider.Trim().ToLowerInvariant();

        if (provider == "sqlite")
        {
            DungSqlite();
            return;
        }

        // Người dùng khai báo chuỗi kết nối cụ thể -> tôn trọng tuyệt đối, không tự lùi về SQLite.
        var configured = AppSettings.ConnectionString;
        if (!string.IsNullOrWhiteSpace(configured))
        {
            DungSqlServer(configured);
            return;
        }

        if (DoTimSqlServer())
        {
            return;
        }

        if (provider == "sqlserver")
        {
            throw new InvalidOperationException(MoTaLoiKhongThaySqlServer(choPhepSqlite: true));
        }

        // Provider = Auto: không có SQL Server thì dùng SQLite để ứng dụng vẫn chạy được.
        DungSqlite();
        DaDungDuPhong = true;
    }

    /// <summary>Mở một kết nối mới đã sẵn sàng dùng. Người gọi chịu trách nhiệm Dispose.</summary>
    public static DbConnection Open()
    {
        DbConnection connection = Loai == LoaiCsdl.SqlServer
            ? new SqlConnection(ConnectionString)
            : new SqliteConnection(ConnectionString);

        connection.Open();
        return connection;
    }

    /// <summary>Tạo chuỗi kết nối SQL Server chuẩn cho một instance + tên CSDL.</summary>
    public static string BuildSqlServerConnectionString(string server, string database) =>
        new SqlConnectionStringBuilder
        {
            DataSource = server,
            InitialCatalog = database,
            IntegratedSecurity = true,
            TrustServerCertificate = true,
            ConnectTimeout = 5,
            MultipleActiveResultSets = true
        }.ConnectionString;

    // ====================================================================
    // SQL Server
    // ====================================================================

    /// <summary>
    /// Thử các instance thông dụng. True nếu kết nối được một cái.
    /// <para>
    /// Các phép thử chạy <b>song song</b>: nếu dò tuần tự thì mỗi instance không tồn tại
    /// phải chờ hết timeout, cộng lại làm ứng dụng treo hàng chục giây khi máy chưa cài SQL Server.
    /// Kết quả vẫn được duyệt theo đúng thứ tự ưu tiên trong <see cref="CandidateServers"/>.
    /// </para>
    /// </summary>
    private static bool DoTimSqlServer()
    {
        var phepThu = CandidateServers
            .Select(server => (Server: server, Task: Task.Run(() => ThuKetNoiMaster(server))))
            .ToArray();

        Task.WaitAll(phepThu.Select(p => (Task)p.Task).ToArray(), TongThoiGianDoToiDa);

        foreach (var (server, task) in phepThu)
        {
            if (!task.IsCompleted)
            {
                _nhatKy.Add($"  - {server}: quá thời gian chờ.");
                continue;
            }

            if (task.Result is { } loi)
            {
                _nhatKy.Add($"  - {server}: {loi}");
                continue;
            }

            try
            {
                DungSqlServer(BuildSqlServerConnectionString(server, AppSettings.DatabaseName));
                return true;
            }
            catch (Exception ex)
            {
                _nhatKy.Add($"  - {server}: {DongDauTien(ex.Message)}");
            }
        }

        return false;
    }

    /// <summary>Thử mở kết nối tới CSDL master. Trả về null nếu thành công, ngược lại là mô tả lỗi.</summary>
    private static string? ThuKetNoiMaster(string server)
    {
        try
        {
            var builder = new SqlConnectionStringBuilder(BuildSqlServerConnectionString(server, "master"))
            {
                ConnectTimeout = ThoiGianChoMoiInstance
            };

            using var connection = new SqlConnection(builder.ConnectionString);
            connection.Open();
            return null;
        }
        catch (Exception ex)
        {
            return DongDauTien(ex.Message);
        }
    }

    /// <summary>Kiểm tra chuỗi kết nối, tạo CSDL nếu cần rồi ghi nhận là chuỗi đang dùng.</summary>
    private static void DungSqlServer(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);

        if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
        {
            builder.InitialCatalog = AppSettings.DatabaseName;
        }

        if (AppSettings.AutoCreate)
        {
            DatabaseInitializer.EnsureSqlServerCreated(builder);
        }

        using (var probe = new SqlConnection(builder.ConnectionString))
        {
            probe.Open();
        }

        Loai = LoaiCsdl.SqlServer;
        ConnectionString = builder.ConnectionString;
        MoTaKetNoi = $"SQL Server: {builder.DataSource} / {builder.InitialCatalog}";
    }

    // ====================================================================
    // SQLite
    // ====================================================================

    /// <summary>Dùng file SQLite cạnh exe, tạo bảng + dữ liệu mẫu nếu file còn trống.</summary>
    private static void DungSqlite()
    {
        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = DuongDanFileSqlite,
            Mode = SqliteOpenMode.ReadWriteCreate,
            ForeignKeys = true
        }.ConnectionString;

        if (AppSettings.AutoCreate)
        {
            DatabaseInitializer.EnsureSqliteCreated(connectionString);
        }

        using (var probe = new SqliteConnection(connectionString))
        {
            probe.Open();
        }

        Loai = LoaiCsdl.Sqlite;
        ConnectionString = connectionString;
        MoTaKetNoi = $"SQLite: {Path.GetFileName(DuongDanFileSqlite)}";
    }

    // ====================================================================
    // Tiện ích
    // ====================================================================

    /// <summary>Soạn thông báo lỗi khi không tìm thấy SQL Server nào.</summary>
    private static string MoTaLoiKhongThaySqlServer(bool choPhepSqlite)
    {
        var thongBao =
            "Không kết nối được tới SQL Server trên máy này." + Environment.NewLine +
            "Đã thử các instance sau:" + Environment.NewLine +
            string.Join(Environment.NewLine, _nhatKy) + Environment.NewLine + Environment.NewLine +
            "Cách khắc phục:" + Environment.NewLine +
            @"  - Mở appsettings.json (cạnh QuanLySP.exe) và điền chuỗi kết nối đúng, ví dụ:" +
            Environment.NewLine +
            @"      ""QuanLyHangHoa"": ""Server=.\SQLEXPRESS;Database=QuanLyHangHoa;Integrated Security=True;TrustServerCertificate=True""";

        if (choPhepSqlite)
        {
            thongBao += Environment.NewLine +
                        @"  - Hoặc đặt ""Provider"": ""Sqlite"" để chạy bằng file CSDL, không cần cài SQL Server.";
        }

        return thongBao;
    }

    private static string DongDauTien(string text)
    {
        var index = text.IndexOf('\n');
        return (index < 0 ? text : text[..index]).Trim();
    }
}
