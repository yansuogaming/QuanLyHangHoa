using System.Data.Common;

namespace QuanLySP.DAL;

/// <summary>
/// Vài hàm mở rộng nhỏ giúp viết ADO.NET gọn và dùng chung được
/// cho cả SqlConnection (SQL Server) lẫn SqliteConnection.
/// </summary>
public static class DbExtensions
{
    /// <summary>Tạo một câu lệnh gắn với kết nối.</summary>
    public static DbCommand TaoLenh(this DbConnection connection, string sql)
    {
        var command = connection.CreateCommand();
        command.CommandText = sql;
        return command;
    }

    /// <summary>Thêm tham số; null hoặc chuỗi rỗng được quy về DBNull.</summary>
    public static DbCommand ThemThamSo(this DbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value switch
        {
            null => DBNull.Value,
            string s when string.IsNullOrWhiteSpace(s) => DBNull.Value,
            string s => s.Trim(),
            _ => value
        };

        command.Parameters.Add(parameter);
        return command;
    }
}
