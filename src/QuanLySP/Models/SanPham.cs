namespace QuanLySP.Models;

/// <summary>
/// Sản phẩm - ánh xạ bảng dbo.SanPham.
/// </summary>
public class SanPham
{
    public int Id { get; set; }

    public int IdLoai { get; set; }

    public string TenSp { get; set; } = string.Empty;

    public decimal Gia { get; set; }

    public int SoLuong { get; set; }

    /// <summary>Tên file ảnh nằm trong thư mục Images cạnh file exe (có thể null).</summary>
    public string? HinhAnh { get; set; }

    public string? NhaSX { get; set; }

    /// <summary>Chỉ để hiển thị trên lưới, lấy từ phép JOIN với LoaiSP.</summary>
    public string TenLoai { get; set; } = string.Empty;

    /// <summary>Thành tiền tồn kho = Gia * SoLuong, hiển thị trên lưới.</summary>
    public decimal ThanhTien => Gia * SoLuong;
}
