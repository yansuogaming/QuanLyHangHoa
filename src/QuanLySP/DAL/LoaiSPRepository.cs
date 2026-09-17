using QuanLySP.Models;

namespace QuanLySP.DAL;

/// <summary>Truy xuất dữ liệu bảng LoaiSP. Câu lệnh viết theo cú pháp chung của SQL Server và SQLite.</summary>
public class LoaiSPRepository
{
    /// <summary>Lấy toàn bộ loại sản phẩm, sắp xếp theo tên.</summary>
    public List<LoaiSP> GetAll()
    {
        const string sql = "SELECT id, TenLoai FROM LoaiSP ORDER BY TenLoai";

        var result = new List<LoaiSP>();

        using var connection = Db.Open();
        using var command = connection.TaoLenh(sql);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            result.Add(new LoaiSP
            {
                Id = reader.GetInt32(0),
                TenLoai = reader.GetString(1)
            });
        }

        return result;
    }

    /// <summary>Thêm một loại sản phẩm mới, trả về id vừa sinh.</summary>
    public int Insert(string tenLoai)
    {
        // SQL Server lấy id bằng mệnh đề OUTPUT, SQLite bằng hàm last_insert_rowid().
        var sql = Db.Loai == LoaiCsdl.SqlServer
            ? "INSERT INTO LoaiSP (TenLoai) OUTPUT INSERTED.id VALUES (@TenLoai)"
            : "INSERT INTO LoaiSP (TenLoai) VALUES (@TenLoai); SELECT last_insert_rowid();";

        using var connection = Db.Open();
        using var command = connection.TaoLenh(sql).ThemThamSo("@TenLoai", tenLoai.Trim());

        return Convert.ToInt32(command.ExecuteScalar());
    }

    /// <summary>Kiểm tra tên loại đã tồn tại chưa (không phân biệt hoa thường).</summary>
    public bool Exists(string tenLoai)
    {
        const string sql = "SELECT COUNT(1) FROM LoaiSP WHERE LOWER(TenLoai) = LOWER(@TenLoai)";

        using var connection = Db.Open();
        using var command = connection.TaoLenh(sql).ThemThamSo("@TenLoai", tenLoai.Trim());

        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }
}
