using System.Globalization;
using QuanLySP.DAL;
using QuanLySP.Helpers;
using QuanLySP.Models;

namespace QuanLySP.Forms;

/// <summary>
/// Form chính: hiển thị, thêm, sửa, xoá sản phẩm.
/// </summary>
public partial class FrmQuanLySanPham : Form
{
    /// <summary>Trạng thái làm việc hiện tại của khung nhập liệu.</summary>
    private enum CheDo
    {
        /// <summary>Không thao tác - chưa chọn dòng nào.</summary>
        Xem,

        /// <summary>Đang nhập một sản phẩm mới.</summary>
        Them,

        /// <summary>Đang sửa sản phẩm được chọn trên lưới.</summary>
        Sua
    }

    /// <summary>Định dạng số tiền kiểu Việt Nam (1.234.567) không phụ thuộc culture của máy.</summary>
    private static readonly NumberFormatInfo DinhDangTien = new()
    {
        NumberGroupSeparator = ".",
        NumberDecimalSeparator = ",",
        NumberDecimalDigits = 0
    };

    private readonly SanPhamRepository _sanPhamRepo = new();
    private readonly LoaiSPRepository _loaiRepo = new();

    private CheDo _cheDo = CheDo.Xem;

    /// <summary>Chặn các sự kiện dây chuyền khi đang nạp dữ liệu vào control.</summary>
    private bool _dangNapDuLieu;

    /// <summary>Tên file ảnh đang gắn với sản phẩm trên khung nhập liệu.</summary>
    private string? _hinhAnhHienTai;

    public FrmQuanLySanPham()
    {
        InitializeComponent();
        ApDungNhanDien();
        TaoCotChoLuoi();
        GanSuKien();
    }

    /// <summary>Gắn tên, phiên bản, tác giả và icon của phần mềm lên giao diện.</summary>
    private void ApDungNhanDien()
    {
        Text = ThongTinUngDung.TieuDeCuaSo;
        lblTacGia.Text = ThongTinUngDung.DongTacGia;

        var icon = ThongTinUngDung.NapIcon();
        if (icon is null)
        {
            return;
        }

        Icon = icon;
        picLogo.Image = icon.ToBitmap();
    }

    // ====================================================================
    // Khởi tạo
    // ====================================================================

    /// <summary>Tạo các cột của DataGridView, gắn với thuộc tính của lớp SanPham.</summary>
    private void TaoCotChoLuoi()
    {
        dgvSanPham.Columns.Clear();

        dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colId",
            DataPropertyName = nameof(SanPham.Id),
            HeaderText = "Mã",
            Width = 60
        });

        dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colTenSp",
            DataPropertyName = nameof(SanPham.TenSp),
            HeaderText = "Tên sản phẩm",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            FillWeight = 160
        });

        dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colTenLoai",
            DataPropertyName = nameof(SanPham.TenLoai),
            HeaderText = "Loại SP",
            Width = 140
        });

        dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colGia",
            DataPropertyName = nameof(SanPham.Gia),
            HeaderText = "Giá (VNĐ)",
            Width = 120,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = "N0",
                FormatProvider = DinhDangTien,
                Alignment = DataGridViewContentAlignment.MiddleRight
            }
        });

        dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colSoLuong",
            DataPropertyName = nameof(SanPham.SoLuong),
            HeaderText = "SL",
            Width = 70,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight
            }
        });

        dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colThanhTien",
            DataPropertyName = nameof(SanPham.ThanhTien),
            HeaderText = "Tồn kho (VNĐ)",
            Width = 130,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = "N0",
                FormatProvider = DinhDangTien,
                Alignment = DataGridViewContentAlignment.MiddleRight,
                ForeColor = Color.FromArgb(33, 82, 156)
            }
        });

        dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colNhaSX",
            DataPropertyName = nameof(SanPham.NhaSX),
            HeaderText = "Nhà sản xuất",
            Width = 140
        });

        dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colHinhAnh",
            DataPropertyName = nameof(SanPham.HinhAnh),
            HeaderText = "Hình ảnh",
            Visible = false
        });
    }

    /// <summary>Gắn toàn bộ trình xử lý sự kiện (thay cho phần designer sinh tự động).</summary>
    private void GanSuKien()
    {
        Load += FrmQuanLySanPham_Load;
        KeyDown += FrmQuanLySanPham_KeyDown;

        dgvSanPham.SelectionChanged += dgvSanPham_SelectionChanged;

        btnTimKiem.Click += (_, _) => NapDanhSach();
        btnTaiLai.Click += (_, _) => TaiLaiTatCa();
        cboLocLoai.SelectedIndexChanged += (_, _) => NapDanhSach();
        txtTimKiem.KeyDown += txtTimKiem_KeyDown;

        btnThemMoi.Click += btnThemMoi_Click;
        btnLuu.Click += btnLuu_Click;
        btnXoa.Click += btnXoa_Click;
        btnHuy.Click += (_, _) => XoaTrangForm(CheDo.Xem);

        btnChonAnh.Click += btnChonAnh_Click;
        btnBoAnh.Click += (_, _) => GanAnh(null);
        btnThemLoai.Click += btnThemLoai_Click;

        txtGia.Leave += (_, _) => txtGia.Text = DocGiaTuO().ToString("N0", DinhDangTien);
    }

    private void FrmQuanLySanPham_Load(object? sender, EventArgs e)
    {
        lblKetNoi.Text = Db.DaDungDuPhong
            ? $"{Db.MoTaKetNoi}  (không tìm thấy SQL Server nên dùng CSDL dự phòng)"
            : Db.MoTaKetNoi;

        NapDanhSachLoai();
        NapDanhSach();
    }

    private void FrmQuanLySanPham_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.F5:
                TaiLaiTatCa();
                break;

            case Keys.Escape:
                XoaTrangForm(CheDo.Xem);
                break;

            case Keys.Delete when dgvSanPham.Focused:
                btnXoa_Click(sender, EventArgs.Empty);
                break;
        }
    }

    // ====================================================================
    // Nạp dữ liệu
    // ====================================================================

    /// <summary>Nạp danh sách loại cho combo lọc và combo nhập liệu.</summary>
    private void NapDanhSachLoai(int? chonId = null)
    {
        try
        {
            var danhSach = _loaiRepo.GetAll();

            _dangNapDuLieu = true;

            // Combo lọc: thêm mục "Tất cả các loại" với Id = 0.
            var danhSachLoc = new List<LoaiSP> { new() { Id = 0, TenLoai = "-- Tất cả các loại --" } };
            danhSachLoc.AddRange(danhSach);

            cboLocLoai.DataSource = danhSachLoc;
            cboLocLoai.DisplayMember = nameof(LoaiSP.TenLoai);
            cboLocLoai.ValueMember = nameof(LoaiSP.Id);
            cboLocLoai.SelectedIndex = 0;

            cboLoai.DataSource = danhSach;
            cboLoai.DisplayMember = nameof(LoaiSP.TenLoai);
            cboLoai.ValueMember = nameof(LoaiSP.Id);
            cboLoai.SelectedIndex = danhSach.Count > 0 ? 0 : -1;

            if (chonId.HasValue && danhSach.Any(l => l.Id == chonId.Value))
            {
                cboLoai.SelectedValue = chonId.Value;
            }
        }
        catch (Exception ex)
        {
            BaoLoi("Không nạp được danh sách loại sản phẩm.", ex);
        }
        finally
        {
            _dangNapDuLieu = false;
        }
    }

    /// <summary>Nạp lưới sản phẩm theo từ khoá và loại đang chọn ở thanh lọc.</summary>
    /// <param name="giuNguyenChiTiet">
    /// True khi phía gọi sẽ tự chọn dòng cần hiển thị (ví dụ sau khi thêm/sửa);
    /// False thì tự chọn dòng đầu tiên cho khung chi tiết luôn khớp với lưới.
    /// </param>
    private void NapDanhSach(bool giuNguyenChiTiet = false)
    {
        if (_dangNapDuLieu)
        {
            return;
        }

        try
        {
            var keyword = txtTimKiem.Text.Trim();
            int? idLoai = cboLocLoai.SelectedValue is int id && id > 0 ? id : null;

            var danhSach = _sanPhamRepo.GetAll(keyword, idLoai);

            _dangNapDuLieu = true;
            dgvSanPham.DataSource = danhSach;
            dgvSanPham.ClearSelection();
            _dangNapDuLieu = false;

            var tongTon = danhSach.Sum(sp => sp.ThanhTien);
            lblTrangThai.Text =
                $"Tổng cộng {danhSach.Count} sản phẩm" +
                $"  |  Giá trị tồn kho: {tongTon.ToString("N0", DinhDangTien)} VNĐ";

            if (!giuNguyenChiTiet)
            {
                ChonDongDauTien();
            }
        }
        catch (Exception ex)
        {
            _dangNapDuLieu = false;
            BaoLoi("Không nạp được danh sách sản phẩm.", ex);
        }
    }

    /// <summary>Xoá bộ lọc và nạp lại toàn bộ dữ liệu.</summary>
    private void TaiLaiTatCa()
    {
        _dangNapDuLieu = true;
        txtTimKiem.Clear();
        if (cboLocLoai.Items.Count > 0)
        {
            cboLocLoai.SelectedIndex = 0;
        }
        _dangNapDuLieu = false;

        NapDanhSachLoai();
        NapDanhSach();
    }

    // ====================================================================
    // Lưới -> khung nhập liệu
    // ====================================================================

    private void dgvSanPham_SelectionChanged(object? sender, EventArgs e)
    {
        if (_dangNapDuLieu)
        {
            return;
        }

        // Đang nhập sản phẩm mới: chỉ bỏ bản nháp khi người dùng thực sự bấm vào lưới,
        // không phải khi lưới tự lấy lại focus.
        if (_cheDo == CheDo.Them && !dgvSanPham.Focused)
        {
            return;
        }

        var sanPham = LaySanPhamDangChon();
        if (sanPham is null)
        {
            return;
        }

        HienThiChiTiet(sanPham);
        DatCheDo(CheDo.Sua);
    }

    /// <summary>Lấy đối tượng SanPham ứng với dòng đang chọn, null nếu không có.</summary>
    private SanPham? LaySanPhamDangChon() =>
        dgvSanPham.SelectedRows.Count > 0
            ? dgvSanPham.SelectedRows[0].DataBoundItem as SanPham
            : null;

    /// <summary>Đổ dữ liệu một sản phẩm lên khung nhập liệu.</summary>
    private void HienThiChiTiet(SanPham sanPham)
    {
        _dangNapDuLieu = true;

        txtMa.Text = sanPham.Id.ToString();
        txtTenSp.Text = sanPham.TenSp;
        txtGia.Text = sanPham.Gia.ToString("N0", DinhDangTien);
        numSoLuong.Value = Math.Clamp(sanPham.SoLuong, (int)numSoLuong.Minimum, (int)numSoLuong.Maximum);
        txtNhaSX.Text = sanPham.NhaSX ?? string.Empty;

        if (cboLoai.Items.Count > 0)
        {
            cboLoai.SelectedValue = sanPham.IdLoai;
        }

        GanAnh(sanPham.HinhAnh);

        _dangNapDuLieu = false;
    }

    /// <summary>Đưa khung nhập liệu về trạng thái trống.</summary>
    private void XoaTrangForm(CheDo cheDoMoi)
    {
        _dangNapDuLieu = true;

        txtMa.Clear();
        txtTenSp.Clear();
        txtGia.Text = "0";
        numSoLuong.Value = 0;
        txtNhaSX.Clear();
        if (cboLoai.Items.Count > 0)
        {
            cboLoai.SelectedIndex = 0;
        }
        GanAnh(null);

        dgvSanPham.ClearSelection();

        _dangNapDuLieu = false;

        DatCheDo(cheDoMoi);
    }

    /// <summary>Cập nhật chế độ làm việc và trạng thái các nút.</summary>
    private void DatCheDo(CheDo cheDo)
    {
        _cheDo = cheDo;

        btnXoa.Enabled = cheDo == CheDo.Sua;
        btnLuu.Text = cheDo == CheDo.Them ? "Thêm" : "Lưu";
        lblChiTiet.Text = cheDo switch
        {
            CheDo.Them => "THÊM SẢN PHẨM MỚI",
            CheDo.Sua => "SỬA SẢN PHẨM",
            _ => "THÔNG TIN SẢN PHẨM"
        };
    }

    // ====================================================================
    // Thêm / sửa / xoá
    // ====================================================================

    private void btnThemMoi_Click(object? sender, EventArgs e)
    {
        XoaTrangForm(CheDo.Them);
        txtTenSp.Focus();
    }

    private void btnLuu_Click(object? sender, EventArgs e)
    {
        if (!KiemTraDuLieu(out var sanPham))
        {
            return;
        }

        try
        {
            if (_cheDo == CheDo.Them)
            {
                var id = _sanPhamRepo.Insert(sanPham);
                NapDanhSach(giuNguyenChiTiet: true);
                ChonDongTheoId(id);
                ThongBao($"Đã thêm sản phẩm \"{sanPham.TenSp}\" (mã {id}).");
            }
            else if (_cheDo == CheDo.Sua)
            {
                if (!int.TryParse(txtMa.Text, out var id))
                {
                    ThongBao("Chưa chọn sản phẩm để sửa.", MessageBoxIcon.Warning);
                    return;
                }

                sanPham.Id = id;

                if (_sanPhamRepo.Update(sanPham))
                {
                    NapDanhSach(giuNguyenChiTiet: true);
                    ChonDongTheoId(id);
                    ThongBao($"Đã cập nhật sản phẩm mã {id}.");
                }
                else
                {
                    ThongBao("Sản phẩm không còn tồn tại, có thể đã bị xoá.", MessageBoxIcon.Warning);
                    NapDanhSach();
                }
            }
            else
            {
                ThongBao("Hãy bấm \"Thêm mới\" hoặc chọn một sản phẩm trên danh sách trước khi lưu.",
                    MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            BaoLoi("Lưu sản phẩm thất bại.", ex);
        }
    }

    private void btnXoa_Click(object? sender, EventArgs e)
    {
        var sanPham = LaySanPhamDangChon();
        if (sanPham is null)
        {
            ThongBao("Hãy chọn một sản phẩm trên danh sách để xoá.", MessageBoxIcon.Information);
            return;
        }

        var traLoi = MessageBox.Show(
            this,
            $"Xoá sản phẩm \"{sanPham.TenSp}\" (mã {sanPham.Id})?{Environment.NewLine}Thao tác này không thể hoàn tác.",
            "Xác nhận xoá",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (traLoi != DialogResult.Yes)
        {
            return;
        }

        try
        {
            if (_sanPhamRepo.Delete(sanPham.Id))
            {
                NapDanhSach();
                ThongBao($"Đã xoá sản phẩm mã {sanPham.Id}.");
            }
            else
            {
                ThongBao("Sản phẩm không còn tồn tại.", MessageBoxIcon.Warning);
                NapDanhSach();
            }
        }
        catch (Exception ex)
        {
            BaoLoi("Xoá sản phẩm thất bại.", ex);
        }
    }

    /// <summary>
    /// Chọn dòng đầu tiên trên lưới (DataGridView vốn luôn tự đặt con trỏ về dòng đầu,
    /// nên làm việc này tường minh để khung chi tiết luôn khớp với dòng đang sáng).
    /// Lưới rỗng thì làm trống khung chi tiết.
    /// </summary>
    private void ChonDongDauTien()
    {
        if (dgvSanPham.Rows.Count == 0)
        {
            XoaTrangForm(CheDo.Xem);
            return;
        }

        var dong = dgvSanPham.Rows[0];

        _dangNapDuLieu = true;
        dong.Selected = true;
        dgvSanPham.CurrentCell = dong.Cells[1];
        _dangNapDuLieu = false;

        if (dong.DataBoundItem is SanPham sanPham)
        {
            HienThiChiTiet(sanPham);
            DatCheDo(CheDo.Sua);
        }
    }

    /// <summary>Đưa con trỏ lưới về đúng dòng có mã chỉ định (sau khi thêm/sửa).</summary>
    private void ChonDongTheoId(int id)
    {
        foreach (DataGridViewRow row in dgvSanPham.Rows)
        {
            if (row.DataBoundItem is SanPham sp && sp.Id == id)
            {
                _dangNapDuLieu = true;
                row.Selected = true;
                dgvSanPham.CurrentCell = row.Cells[1];
                _dangNapDuLieu = false;

                HienThiChiTiet(sp);
                DatCheDo(CheDo.Sua);
                return;
            }
        }
    }

    // ====================================================================
    // Kiểm tra dữ liệu nhập
    // ====================================================================

    /// <summary>
    /// Kiểm tra dữ liệu trên khung nhập liệu.
    /// Trả về true và đối tượng <paramref name="sanPham"/> khi hợp lệ.
    /// </summary>
    private bool KiemTraDuLieu(out SanPham sanPham)
    {
        sanPham = new SanPham();

        if (string.IsNullOrWhiteSpace(txtTenSp.Text))
        {
            ThongBao("Vui lòng nhập tên sản phẩm.", MessageBoxIcon.Warning);
            txtTenSp.Focus();
            return false;
        }

        if (cboLoai.SelectedValue is not int idLoai || idLoai <= 0)
        {
            ThongBao("Vui lòng chọn loại sản phẩm. Nếu chưa có loại nào, hãy bấm nút \"+\" để thêm.",
                MessageBoxIcon.Warning);
            cboLoai.Focus();
            return false;
        }

        if (!ThuDocGia(out var gia))
        {
            ThongBao("Giá không hợp lệ. Chỉ nhập số, ví dụ: 1500000 hoặc 1.500.000.", MessageBoxIcon.Warning);
            txtGia.Focus();
            txtGia.SelectAll();
            return false;
        }

        sanPham.TenSp = txtTenSp.Text.Trim();
        sanPham.IdLoai = idLoai;
        sanPham.Gia = gia;
        sanPham.SoLuong = (int)numSoLuong.Value;
        sanPham.NhaSX = txtNhaSX.Text.Trim();
        sanPham.HinhAnh = _hinhAnhHienTai;

        return true;
    }

    /// <summary>Đọc giá từ ô nhập, chấp nhận cả dạng có dấu phân cách nghìn.</summary>
    private bool ThuDocGia(out decimal gia)
    {
        var raw = txtGia.Text
            .Replace(".", string.Empty)
            .Replace(",", string.Empty)
            .Replace(" ", string.Empty)
            .Replace("đ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        if (raw.Length == 0)
        {
            gia = 0;
            return true;
        }

        var hopLe = decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out gia);
        return hopLe && gia >= 0;
    }

    /// <summary>Đọc giá, trả về 0 nếu không hợp lệ (dùng khi định dạng lại ô nhập).</summary>
    private decimal DocGiaTuO() => ThuDocGia(out var gia) ? gia : 0;

    // ====================================================================
    // Ảnh và loại sản phẩm
    // ====================================================================

    private void btnChonAnh_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Chọn ảnh sản phẩm",
            Filter = ImageStore.Filter,
            CheckFileExists = true
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            GanAnh(ImageStore.Save(dialog.FileName));
        }
        catch (Exception ex)
        {
            BaoLoi("Không sao chép được ảnh vào thư mục Images.", ex);
        }
    }

    /// <summary>Gắn ảnh (theo tên file) lên PictureBox, null để bỏ ảnh.</summary>
    private void GanAnh(string? tenFile)
    {
        _hinhAnhHienTai = string.IsNullOrWhiteSpace(tenFile) ? null : tenFile;

        var anhCu = picHinhAnh.Image;
        picHinhAnh.Image = ImageStore.Load(_hinhAnhHienTai);
        anhCu?.Dispose();
    }

    private void btnThemLoai_Click(object? sender, EventArgs e)
    {
        using var dialog = new FrmThemLoai();
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            if (_loaiRepo.Exists(dialog.TenLoai))
            {
                ThongBao($"Loại \"{dialog.TenLoai}\" đã tồn tại.", MessageBoxIcon.Warning);
                return;
            }

            var id = _loaiRepo.Insert(dialog.TenLoai);
            NapDanhSachLoai(id);
            ThongBao($"Đã thêm loại \"{dialog.TenLoai}\".");
        }
        catch (Exception ex)
        {
            BaoLoi("Thêm loại sản phẩm thất bại.", ex);
        }
    }

    // ====================================================================
    // Tiện ích
    // ====================================================================

    private void txtTimKiem_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter)
        {
            return;
        }

        e.SuppressKeyPress = true;
        NapDanhSach();
    }

    private void ThongBao(string noiDung, MessageBoxIcon icon = MessageBoxIcon.Information) =>
        MessageBox.Show(this, noiDung, "Quản lý sản phẩm", MessageBoxButtons.OK, icon);

    private void BaoLoi(string tieuDe, Exception ex) =>
        MessageBox.Show(
            this,
            $"{tieuDe}{Environment.NewLine}{Environment.NewLine}Chi tiết: {ex.Message}",
            "Lỗi",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
}
