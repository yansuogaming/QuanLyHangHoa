using System.Data.Common;
using QuanLySP.Models;

namespace QuanLySP.DAL;

/// <summary>
/// Truy xuất dữ liệu bảng SanPham (CRUD đầy đủ).
/// Câu lệnh viết theo cú pháp chung của SQL Server và SQLite; chỗ nào khác nhau
/// thì rẽ nhánh theo <see cref="Db.Loai"/>.
/// </summary>
public class SanPhamRepository
{
    private const string SelectBase = """
        SELECT sp.id, sp.IdLoai, sp.TenSp, sp.Gia, sp.SoLuong, sp.HinhAnh, sp.NhaSX, l.TenLoai
        FROM SanPham sp
        INNER JOIN LoaiSP l ON l.id = sp.IdLoai
        """;

    private const string ColumnList = "IdLoai, TenSp, Gia, SoLuong, HinhAnh, NhaSX";
    private const string ValueList = "@IdLoai, @TenSp, @Gia, @SoLuong, @HinhAnh, @NhaSX";

    /// <summary>
    /// Lấy danh sách sản phẩm, có thể lọc theo từ khoá (tên SP / nhà sản xuất)
    /// và theo loại sản phẩm.
    /// </summary>
    /// <param name="keyword">Từ khoá tìm kiếm, null hoặc rỗng = không lọc.</param>
    /// <param name="idLoai">Id loại sản phẩm, null = tất cả các loại.</param>
    public List<SanPham> GetAll(string? keyword = null, int? idLoai = null)
    {
        var coKeyword = !string.IsNullOrWhiteSpace(keyword);

        var sql = SelectBase + " WHERE 1 = 1";

        if (coKeyword)
        {
            // COALESCE chạy được trên cả SQL Server lẫn SQLite (khác với ISNULL/IFNULL).
            sql += " AND (LOWER(sp.TenSp) LIKE @Keyword OR LOWER(COALESCE(sp.NhaSX, '')) LIKE @Keyword)";
        }

        if (idLoai.HasValue)
        {
            sql += " AND sp.IdLoai = @IdLoai";
        }

        sql += " ORDER BY sp.id DESC";

        var result = new List<SanPham>();

        using var connection = Db.Open();
        using var command = connection.TaoLenh(sql);

        if (coKeyword)
        {
            command.ThemThamSo("@Keyword", $"%{keyword!.Trim().ToLowerInvariant()}%");
        }

        if (idLoai.HasValue)
        {
            command.ThemThamSo("@IdLoai", idLoai.Value);
        }

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(Map(reader));
        }

        return result;
    }

    /// <summary>Lấy một sản phẩm theo id, trả về null nếu không tồn tại.</summary>
    public SanPham? GetById(int id)
    {
        using var connection = Db.Open();
        using var command = connection.TaoLenh(SelectBase + " WHERE sp.id = @Id").ThemThamSo("@Id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    /// <summary>Thêm sản phẩm mới, trả về id vừa sinh.</summary>
    public int Insert(SanPham sanPham)
    {
        var sql = Db.Loai == LoaiCsdl.SqlServer
            ? $"INSERT INTO SanPham ({ColumnList}) OUTPUT INSERTED.id VALUES ({ValueList})"
            : $"INSERT INTO SanPham ({ColumnList}) VALUES ({ValueList}); SELECT last_insert_rowid();";

        using var connection = Db.Open();
        using var command = connection.TaoLenh(sql);
        AddParameters(command, sanPham);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    /// <summary>Cập nhật sản phẩm, trả về true nếu có dòng bị ảnh hưởng.</summary>
    public bool Update(SanPham sanPham)
    {
        const string sql = """
            UPDATE SanPham
            SET IdLoai  = @IdLoai,
                TenSp   = @TenSp,
                Gia     = @Gia,
                SoLuong = @SoLuong,
                HinhAnh = @HinhAnh,
                NhaSX   = @NhaSX
            WHERE id = @Id
            """;

        using var connection = Db.Open();
        using var command = connection.TaoLenh(sql);
        AddParameters(command, sanPham);
        command.ThemThamSo("@Id", sanPham.Id);

        return command.ExecuteNonQuery() > 0;
    }

    /// <summary>Xoá sản phẩm theo id, trả về true nếu có dòng bị xoá.</summary>
    public bool Delete(int id)
    {
        using var connection = Db.Open();
        using var command = connection.TaoLenh("DELETE FROM SanPham WHERE id = @Id").ThemThamSo("@Id", id);

        return command.ExecuteNonQuery() > 0;
    }

    /// <summary>Gắn tham số dùng chung cho câu lệnh INSERT và UPDATE.</summary>
    private static void AddParameters(DbCommand command, SanPham sanPham)
    {
        command.ThemThamSo("@IdLoai", sanPham.IdLoai)
               .ThemThamSo("@TenSp", sanPham.TenSp.Trim())
               .ThemThamSo("@Gia", sanPham.Gia)
               .ThemThamSo("@SoLuong", sanPham.SoLuong)
               .ThemThamSo("@HinhAnh", sanPham.HinhAnh)
               .ThemThamSo("@NhaSX", sanPham.NhaSX);
    }

    /// <summary>Đọc một dòng dữ liệu thành đối tượng SanPham.</summary>
    private static SanPham Map(DbDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        IdLoai = reader.GetInt32(1),
        TenSp = reader.GetString(2),
        Gia = reader.GetDecimal(3),
        SoLuong = reader.GetInt32(4),
        HinhAnh = reader.IsDBNull(5) ? null : reader.GetString(5),
        NhaSX = reader.IsDBNull(6) ? null : reader.GetString(6),
        TenLoai = reader.GetString(7)
    };
}
