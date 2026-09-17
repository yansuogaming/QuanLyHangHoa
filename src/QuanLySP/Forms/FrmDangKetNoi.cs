using QuanLySP.DAL;
using QuanLySP.Helpers;

namespace QuanLySP.Forms;

/// <summary>
/// Màn hình chờ hiển thị trong lúc dò SQL Server / tạo cơ sở dữ liệu.
/// Việc khởi tạo chạy trên luồng nền nên cửa sổ không bị "đơ".
/// </summary>
public class FrmDangKetNoi : Form
{
    /// <summary>Lỗi gặp phải khi khởi tạo, null nếu thành công.</summary>
    public Exception? Loi { get; private set; }

    public FrmDangKetNoi()
    {
        Text = ThongTinUngDung.Ten;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(430, 158);
        BackColor = Color.FromArgb(33, 82, 156);
        ShowInTaskbar = true;
        ControlBox = false;

        var icon = ThongTinUngDung.NapIcon();
        if (icon is not null)
        {
            Icon = icon;

            Controls.Add(new PictureBox
            {
                Image = icon.ToBitmap(),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(28, 24),
                Size = new Size(54, 54),
                BackColor = Color.Transparent
            });
        }

        Controls.Add(new Label
        {
            AutoSize = true,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            Location = new Point(94, 26),
            Text = ThongTinUngDung.Ten.ToUpperInvariant()
        });

        Controls.Add(new Label
        {
            AutoSize = true,
            ForeColor = Color.FromArgb(188, 209, 240),
            Font = new Font("Segoe UI", 8.5F),
            Location = new Point(96, 55),
            Text = ThongTinUngDung.DongTacGia
        });

        Controls.Add(new Label
        {
            AutoSize = true,
            ForeColor = Color.FromArgb(206, 222, 245),
            Font = new Font("Segoe UI", 9F),
            Location = new Point(30, 94),
            Text = "Đang kết nối cơ sở dữ liệu, vui lòng đợi..."
        });

        Controls.Add(new ProgressBar
        {
            Location = new Point(30, 120),
            Size = new Size(370, 12),
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 30
        });

        Shown += async (_, _) =>
        {
            try
            {
                // Chạy trên luồng nền: dò SQL Server có thể mất vài giây.
                await Task.Run(Db.Initialize);
            }
            catch (Exception ex)
            {
                Loi = ex;
            }

            Close();
        };
    }
}
