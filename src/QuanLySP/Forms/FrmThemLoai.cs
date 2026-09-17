namespace QuanLySP.Forms;

/// <summary>
/// Hộp thoại nhỏ để nhập nhanh một loại sản phẩm mới.
/// Form này được dựng hoàn toàn bằng code nên không cần file Designer.
/// </summary>
public class FrmThemLoai : Form
{
    private readonly TextBox _txtTenLoai;

    /// <summary>Tên loại người dùng đã nhập (đã Trim).</summary>
    public string TenLoai => _txtTenLoai.Text.Trim();

    public FrmThemLoai()
    {
        Text = "Thêm loại sản phẩm";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(380, 130);
        Font = new Font("Segoe UI", 9F);
        BackColor = Color.White;

        var lbl = new Label
        {
            AutoSize = true,
            Location = new Point(16, 20),
            Text = "Tên loại sản phẩm:"
        };

        _txtTenLoai = new TextBox
        {
            Location = new Point(16, 44),
            Width = 348,
            MaxLength = 100
        };

        var btnOk = new Button
        {
            Location = new Point(176, 82),
            Size = new Size(90, 32),
            Text = "Đồng ý",
            DialogResult = DialogResult.OK,
            BackColor = Color.FromArgb(33, 82, 156),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnOk.FlatAppearance.BorderSize = 0;

        var btnCancel = new Button
        {
            Location = new Point(274, 82),
            Size = new Size(90, 32),
            Text = "Huỷ",
            DialogResult = DialogResult.Cancel
        };

        Controls.AddRange(new Control[] { lbl, _txtTenLoai, btnOk, btnCancel });

        AcceptButton = btnOk;
        CancelButton = btnCancel;

        // Không cho đóng bằng OK khi chưa nhập gì.
        btnOk.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(_txtTenLoai.Text))
            {
                MessageBox.Show(this, "Vui lòng nhập tên loại sản phẩm.", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                _txtTenLoai.Focus();
            }
        };
    }
}
