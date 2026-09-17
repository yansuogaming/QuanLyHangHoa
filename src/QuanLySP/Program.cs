using QuanLySP.Forms;

namespace QuanLySP;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Màn hình chờ: dò SQL Server, nếu không có thì dùng SQLite, rồi tạo CSDL nếu cần.
        Exception? loi;
        using (var manHinhCho = new FrmDangKetNoi())
        {
            Application.Run(manHinhCho);
            loi = manHinhCho.Loi;
        }

        if (loi is not null)
        {
            MessageBox.Show(
                loi.Message,
                "Không kết nối được cơ sở dữ liệu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        Application.Run(new FrmQuanLySanPham());
    }
}
