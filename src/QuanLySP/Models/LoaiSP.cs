namespace QuanLySP.Models;

/// <summary>
/// Loại sản phẩm - ánh xạ bảng dbo.LoaiSP.
/// </summary>
public class LoaiSP
{
    public int Id { get; set; }

    public string TenLoai { get; set; } = string.Empty;

    /// <summary>Dùng cho ComboBox (DisplayMember mặc định gọi ToString()).</summary>
    public override string ToString() => TenLoai;
}
