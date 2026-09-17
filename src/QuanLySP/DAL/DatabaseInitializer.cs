using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace QuanLySP.DAL;

/// <summary>
/// Tự tạo CSDL QuanLyHangHoa (bảng + dữ liệu mẫu) bằng script SQL được nhúng trong exe,
/// cho cả SQL Server lẫn SQLite. Nhờ vậy ứng dụng chạy được ngay lần đầu.
/// </summary>
public static class DatabaseInitializer
{
    private const string ResourceSqlServer = "QuanLySP.Database.QuanLyHangHoa.sql";
    private const string ResourceSqlite = "QuanLySP.Database.QuanLyHangHoa.Sqlite.sql";

    /// <summary>Đánh dấu ranh giới giữa phần tạo bảng và phần dữ liệu mẫu trong script SQLite.</summary>
    private const string SeedMarker = "-- @SEED";

    /// <summary>Dòng chỉ chứa từ khoá GO là ranh giới giữa các batch của SQL Server.</summary>
    private static readonly Regex BatchSeparator =
        new(@"^\s*GO\s*(?:--.*)?$", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

    // ====================================================================
    // SQL Server
    // ====================================================================

    /// <summary>Bảo đảm CSDL và các bảng đã tồn tại. Không làm gì nếu mọi thứ đã sẵn sàng.</summary>
    public static void EnsureSqlServerCreated(SqlConnectionStringBuilder target)
    {
        var master = new SqlConnectionStringBuilder(target.ConnectionString)
        {
            InitialCatalog = "master"
        };

        using var connection = new SqlConnection(master.ConnectionString);
        connection.Open();

        if (SqlServerDaSanSang(connection, target))
        {
            return;
        }

        foreach (var batch in TachBatch(DocScript(ResourceSqlServer)))
        {
            using var command = new SqlCommand(batch, connection) { CommandTimeout = 120 };
            command.ExecuteNonQuery();
        }
    }

    /// <summary>CSDL đã tồn tại và có đủ hai bảng LoaiSP, SanPham?</summary>
    private static bool SqlServerDaSanSang(SqlConnection masterConnection, SqlConnectionStringBuilder target)
    {
        using (var exists = new SqlCommand("SELECT CASE WHEN DB_ID(@db) IS NULL THEN 0 ELSE 1 END", masterConnection))
        {
            exists.Parameters.AddWithValue("@db", target.InitialCatalog);
            if (Convert.ToInt32(exists.ExecuteScalar()) == 0)
            {
                return false;
            }
        }

        const string checkTables = """
            SELECT CASE WHEN OBJECT_ID(N'dbo.LoaiSP', N'U') IS NOT NULL
                         AND OBJECT_ID(N'dbo.SanPham', N'U') IS NOT NULL
                    THEN 1 ELSE 0 END
            """;

        using var connection = new SqlConnection(target.ConnectionString);
        connection.Open();

        using var command = new SqlCommand(checkTables, connection);
        return Convert.ToInt32(command.ExecuteScalar()) == 1;
    }

    // ====================================================================
    // SQLite
    // ====================================================================

    /// <summary>
    /// Tạo file CSDL SQLite nếu chưa có, tạo bảng, và nạp dữ liệu mẫu khi bảng SanPham còn trống.
    /// </summary>
    public static void EnsureSqliteCreated(string connectionString)
    {
        var script = DocScript(ResourceSqlite);

        var viTri = script.IndexOf(SeedMarker, StringComparison.Ordinal);
        var phanTaoBang = viTri < 0 ? script : script[..viTri];
        var phanDuLieuMau = viTri < 0 ? string.Empty : script[viTri..];

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        ChayLenh(connection, phanTaoBang);

        if (string.IsNullOrWhiteSpace(phanDuLieuMau) || !BangSanPhamTrong(connection))
        {
            return;
        }

        using var transaction = connection.BeginTransaction();
        ChayLenh(connection, phanDuLieuMau, transaction);
        transaction.Commit();
    }

    private static bool BangSanPhamTrong(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(1) FROM SanPham";
        return Convert.ToInt64(command.ExecuteScalar()) == 0;
    }

    private static void ChayLenh(SqliteConnection connection, string sql, SqliteTransaction? transaction = null)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        command.ExecuteNonQuery();
    }

    // ====================================================================
    // Dùng chung
    // ====================================================================

    /// <summary>Đọc nội dung một script SQL được nhúng trong assembly.</summary>
    private static string DocScript(string resourceName)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Không tìm thấy script SQL được nhúng ({resourceName}).");

        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        return reader.ReadToEnd();
    }

    /// <summary>Tách script SQL Server thành từng batch theo từ khoá GO.</summary>
    private static List<string> TachBatch(string script) =>
        BatchSeparator
            .Split(script)
            .Where(batch => !string.IsNullOrWhiteSpace(batch))
            .Select(batch => batch.Trim())
            .ToList();
}
