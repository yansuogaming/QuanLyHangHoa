namespace QuanLySP.Forms;

partial class FrmQuanLySanPham
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlHeader;
    private PictureBox picLogo;
    private Label lblTieuDe;
    private Label lblMoTa;
    private Label lblTacGia;

    private Panel pnlFilter;
    private Label lblTimKiem;
    private TextBox txtTimKiem;
    private Label lblLocLoai;
    private ComboBox cboLocLoai;
    private Button btnTimKiem;
    private Button btnTaiLai;

    private DataGridView dgvSanPham;

    private Panel pnlChiTiet;
    private Label lblChiTiet;
    private PictureBox picHinhAnh;
    private Button btnChonAnh;
    private Button btnBoAnh;
    private Label lblMa;
    private TextBox txtMa;
    private Label lblTenSp;
    private TextBox txtTenSp;
    private Label lblLoai;
    private ComboBox cboLoai;
    private Button btnThemLoai;
    private Label lblGia;
    private TextBox txtGia;
    private Label lblSoLuong;
    private NumericUpDown numSoLuong;
    private Label lblNhaSX;
    private TextBox txtNhaSX;
    private Button btnThemMoi;
    private Button btnLuu;
    private Button btnXoa;
    private Button btnHuy;

    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblTrangThai;
    private ToolStripStatusLabel lblKetNoi;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        pnlHeader = new Panel();
        picLogo = new PictureBox();
        lblTieuDe = new Label();
        lblMoTa = new Label();
        lblTacGia = new Label();
        pnlFilter = new Panel();
        lblTimKiem = new Label();
        txtTimKiem = new TextBox();
        lblLocLoai = new Label();
        cboLocLoai = new ComboBox();
        btnTimKiem = new Button();
        btnTaiLai = new Button();
        dgvSanPham = new DataGridView();
        pnlChiTiet = new Panel();
        lblChiTiet = new Label();
        picHinhAnh = new PictureBox();
        btnChonAnh = new Button();
        btnBoAnh = new Button();
        lblMa = new Label();
        txtMa = new TextBox();
        lblTenSp = new Label();
        txtTenSp = new TextBox();
        lblLoai = new Label();
        cboLoai = new ComboBox();
        btnThemLoai = new Button();
        lblGia = new Label();
        txtGia = new TextBox();
        lblSoLuong = new Label();
        numSoLuong = new NumericUpDown();
        lblNhaSX = new Label();
        txtNhaSX = new TextBox();
        btnThemMoi = new Button();
        btnLuu = new Button();
        btnXoa = new Button();
        btnHuy = new Button();
        statusStrip = new StatusStrip();
        lblTrangThai = new ToolStripStatusLabel();
        lblKetNoi = new ToolStripStatusLabel();

        ((System.ComponentModel.ISupportInitialize)dgvSanPham).BeginInit();
        ((System.ComponentModel.ISupportInitialize)picHinhAnh).BeginInit();
        ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
        numSoLuong.BeginInit();
        pnlHeader.SuspendLayout();
        pnlFilter.SuspendLayout();
        pnlChiTiet.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();

        // ----------------------------------------------------------------
        // pnlHeader - dai tieu de tren cung
        // ----------------------------------------------------------------
        pnlHeader.BackColor = Color.FromArgb(33, 82, 156);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Height = 66;
        pnlHeader.Controls.Add(lblTacGia);
        pnlHeader.Controls.Add(lblMoTa);
        pnlHeader.Controls.Add(lblTieuDe);
        pnlHeader.Controls.Add(picLogo);

        picLogo.Location = new Point(18, 13);
        picLogo.Size = new Size(40, 40);
        picLogo.SizeMode = PictureBoxSizeMode.Zoom;
        picLogo.BackColor = Color.Transparent;

        lblTieuDe.AutoSize = true;
        lblTieuDe.ForeColor = Color.White;
        lblTieuDe.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblTieuDe.Location = new Point(68, 10);
        lblTieuDe.Text = "QUẢN LÝ SẢN PHẨM";

        lblMoTa.AutoSize = true;
        lblMoTa.ForeColor = Color.FromArgb(206, 222, 245);
        lblMoTa.Font = new Font("Segoe UI", 9F);
        lblMoTa.Location = new Point(71, 41);
        lblMoTa.Text = "Cơ sở dữ liệu QuanLyHangHoa — bảng LoaiSP và SanPham";

        // Dùng Dock thay cho Anchor: Anchor tính khoảng cách theo bề rộng panel tại thời
        // điểm khởi tạo (chưa giãn theo form) nên nhãn sẽ bị đẩy ra ngoài vùng nhìn thấy.
        lblTacGia.AutoSize = false;
        lblTacGia.Dock = DockStyle.Right;
        lblTacGia.Width = 340;
        lblTacGia.ForeColor = Color.FromArgb(188, 209, 240);
        lblTacGia.Font = new Font("Segoe UI", 8.5F);
        lblTacGia.Padding = new Padding(0, 0, 18, 12);
        lblTacGia.TextAlign = ContentAlignment.BottomRight;
        lblTacGia.Text = "Phiên bản 1.0  •  Build: Yansuo";

        // ----------------------------------------------------------------
        // pnlFilter - thanh tim kiem / loc
        // ----------------------------------------------------------------
        pnlFilter.Dock = DockStyle.Top;
        pnlFilter.Height = 56;
        pnlFilter.BackColor = Color.FromArgb(244, 246, 250);
        pnlFilter.Controls.Add(btnTaiLai);
        pnlFilter.Controls.Add(btnTimKiem);
        pnlFilter.Controls.Add(cboLocLoai);
        pnlFilter.Controls.Add(lblLocLoai);
        pnlFilter.Controls.Add(txtTimKiem);
        pnlFilter.Controls.Add(lblTimKiem);

        lblTimKiem.AutoSize = true;
        lblTimKiem.Location = new Point(18, 19);
        lblTimKiem.Text = "Tìm kiếm:";

        txtTimKiem.Location = new Point(92, 15);
        txtTimKiem.Width = 240;
        txtTimKiem.PlaceholderText = "Tên sản phẩm hoặc nhà sản xuất...";

        lblLocLoai.AutoSize = true;
        lblLocLoai.Location = new Point(352, 19);
        lblLocLoai.Text = "Loại:";

        cboLocLoai.Location = new Point(392, 15);
        cboLocLoai.Width = 200;
        cboLocLoai.DropDownStyle = ComboBoxStyle.DropDownList;

        btnTimKiem.Location = new Point(610, 14);
        btnTimKiem.Size = new Size(96, 28);
        btnTimKiem.Text = "Tìm";
        btnTimKiem.BackColor = Color.FromArgb(33, 82, 156);
        btnTimKiem.ForeColor = Color.White;
        btnTimKiem.FlatStyle = FlatStyle.Flat;
        btnTimKiem.FlatAppearance.BorderSize = 0;

        btnTaiLai.Location = new Point(716, 14);
        btnTaiLai.Size = new Size(110, 28);
        btnTaiLai.Text = "Tải lại (F5)";

        // ----------------------------------------------------------------
        // dgvSanPham - luoi danh sach san pham
        // ----------------------------------------------------------------
        dgvSanPham.Dock = DockStyle.Fill;
        dgvSanPham.AllowUserToAddRows = false;
        dgvSanPham.AllowUserToDeleteRows = false;
        dgvSanPham.AllowUserToResizeRows = false;
        dgvSanPham.ReadOnly = true;
        dgvSanPham.MultiSelect = false;
        dgvSanPham.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvSanPham.AutoGenerateColumns = false;
        dgvSanPham.BackgroundColor = Color.White;
        dgvSanPham.BorderStyle = BorderStyle.None;
        dgvSanPham.RowHeadersVisible = false;
        dgvSanPham.RowTemplate.Height = 30;
        dgvSanPham.ColumnHeadersHeight = 36;
        dgvSanPham.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvSanPham.EnableHeadersVisualStyles = false;
        dgvSanPham.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(232, 238, 248);
        dgvSanPham.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        dgvSanPham.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
        dgvSanPham.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 250, 252);

        // ----------------------------------------------------------------
        // pnlChiTiet - khung nhap lieu ben phai
        // ----------------------------------------------------------------
        pnlChiTiet.Dock = DockStyle.Right;
        pnlChiTiet.Width = 392;
        pnlChiTiet.BackColor = Color.FromArgb(250, 251, 253);
        pnlChiTiet.BorderStyle = BorderStyle.FixedSingle;
        pnlChiTiet.Controls.Add(lblChiTiet);
        pnlChiTiet.Controls.Add(picHinhAnh);
        pnlChiTiet.Controls.Add(btnChonAnh);
        pnlChiTiet.Controls.Add(btnBoAnh);
        pnlChiTiet.Controls.Add(lblMa);
        pnlChiTiet.Controls.Add(txtMa);
        pnlChiTiet.Controls.Add(lblTenSp);
        pnlChiTiet.Controls.Add(txtTenSp);
        pnlChiTiet.Controls.Add(lblLoai);
        pnlChiTiet.Controls.Add(cboLoai);
        pnlChiTiet.Controls.Add(btnThemLoai);
        pnlChiTiet.Controls.Add(lblGia);
        pnlChiTiet.Controls.Add(txtGia);
        pnlChiTiet.Controls.Add(lblSoLuong);
        pnlChiTiet.Controls.Add(numSoLuong);
        pnlChiTiet.Controls.Add(lblNhaSX);
        pnlChiTiet.Controls.Add(txtNhaSX);
        pnlChiTiet.Controls.Add(btnThemMoi);
        pnlChiTiet.Controls.Add(btnLuu);
        pnlChiTiet.Controls.Add(btnXoa);
        pnlChiTiet.Controls.Add(btnHuy);

        lblChiTiet.AutoSize = true;
        lblChiTiet.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblChiTiet.ForeColor = Color.FromArgb(33, 82, 156);
        lblChiTiet.Location = new Point(16, 12);
        lblChiTiet.Text = "THÔNG TIN SẢN PHẨM";

        picHinhAnh.Location = new Point(16, 40);
        picHinhAnh.Size = new Size(352, 186);
        picHinhAnh.BorderStyle = BorderStyle.FixedSingle;
        picHinhAnh.BackColor = Color.White;
        picHinhAnh.SizeMode = PictureBoxSizeMode.Zoom;

        btnChonAnh.Location = new Point(16, 232);
        btnChonAnh.Size = new Size(172, 30);
        btnChonAnh.Text = "Chọn ảnh...";

        btnBoAnh.Location = new Point(196, 232);
        btnBoAnh.Size = new Size(172, 30);
        btnBoAnh.Text = "Bỏ ảnh";

        lblMa.AutoSize = true;
        lblMa.Location = new Point(16, 285);
        lblMa.Text = "Mã SP:";

        txtMa.Location = new Point(120, 281);
        txtMa.Width = 248;
        txtMa.ReadOnly = true;
        txtMa.BackColor = Color.FromArgb(238, 240, 244);
        txtMa.TabStop = false;

        lblTenSp.AutoSize = true;
        lblTenSp.Location = new Point(16, 320);
        lblTenSp.Text = "Tên SP (*):";

        txtTenSp.Location = new Point(120, 316);
        txtTenSp.Width = 248;
        txtTenSp.MaxLength = 200;

        lblLoai.AutoSize = true;
        lblLoai.Location = new Point(16, 355);
        lblLoai.Text = "Loại SP (*):";

        cboLoai.Location = new Point(120, 351);
        cboLoai.Width = 210;
        cboLoai.DropDownStyle = ComboBoxStyle.DropDownList;

        btnThemLoai.Location = new Point(336, 350);
        btnThemLoai.Size = new Size(32, 25);
        btnThemLoai.Text = "+";
        btnThemLoai.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

        lblGia.AutoSize = true;
        lblGia.Location = new Point(16, 390);
        lblGia.Text = "Giá (VNĐ):";

        txtGia.Location = new Point(120, 386);
        txtGia.Width = 248;
        txtGia.TextAlign = HorizontalAlignment.Right;

        lblSoLuong.AutoSize = true;
        lblSoLuong.Location = new Point(16, 425);
        lblSoLuong.Text = "Số lượng:";

        numSoLuong.Location = new Point(120, 421);
        numSoLuong.Width = 248;
        numSoLuong.Maximum = 1000000;
        numSoLuong.TextAlign = HorizontalAlignment.Right;

        lblNhaSX.AutoSize = true;
        lblNhaSX.Location = new Point(16, 460);
        lblNhaSX.Text = "Nhà SX:";

        txtNhaSX.Location = new Point(120, 456);
        txtNhaSX.Width = 248;
        txtNhaSX.MaxLength = 150;

        btnThemMoi.Location = new Point(16, 504);
        btnThemMoi.Size = new Size(112, 38);
        btnThemMoi.Text = "Thêm mới";

        btnLuu.Location = new Point(136, 504);
        btnLuu.Size = new Size(112, 38);
        btnLuu.Text = "Lưu";
        btnLuu.BackColor = Color.FromArgb(33, 82, 156);
        btnLuu.ForeColor = Color.White;
        btnLuu.FlatStyle = FlatStyle.Flat;
        btnLuu.FlatAppearance.BorderSize = 0;

        btnXoa.Location = new Point(256, 504);
        btnXoa.Size = new Size(112, 38);
        btnXoa.Text = "Xoá";
        btnXoa.BackColor = Color.FromArgb(190, 54, 54);
        btnXoa.ForeColor = Color.White;
        btnXoa.FlatStyle = FlatStyle.Flat;
        btnXoa.FlatAppearance.BorderSize = 0;

        btnHuy.Location = new Point(16, 550);
        btnHuy.Size = new Size(352, 32);
        btnHuy.Text = "Huỷ / Bỏ chọn";

        // ----------------------------------------------------------------
        // statusStrip
        // ----------------------------------------------------------------
        statusStrip.Items.AddRange(new ToolStripItem[] { lblTrangThai, lblKetNoi });
        statusStrip.SizingGrip = false;

        lblTrangThai.Spring = true;
        lblTrangThai.TextAlign = ContentAlignment.MiddleLeft;
        lblTrangThai.Text = "Sẵn sàng";

        lblKetNoi.Text = string.Empty;

        // ----------------------------------------------------------------
        // FrmQuanLySanPham
        // ----------------------------------------------------------------
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1200, 730);
        MinimumSize = new Size(1080, 700);
        Font = new Font("Segoe UI", 9F);
        BackColor = Color.White;
        Controls.Add(dgvSanPham);
        Controls.Add(pnlChiTiet);
        Controls.Add(pnlFilter);
        Controls.Add(pnlHeader);
        Controls.Add(statusStrip);
        KeyPreview = true;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Quản Lý Sản Phẩm";

        ((System.ComponentModel.ISupportInitialize)dgvSanPham).EndInit();
        ((System.ComponentModel.ISupportInitialize)picHinhAnh).EndInit();
        ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
        numSoLuong.EndInit();
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        pnlFilter.ResumeLayout(false);
        pnlFilter.PerformLayout();
        pnlChiTiet.ResumeLayout(false);
        pnlChiTiet.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
