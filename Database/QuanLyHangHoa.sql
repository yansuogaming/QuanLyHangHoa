/* =====================================================================
   QuanLyHangHoa - Script tạo cơ sở dữ liệu quản lý hàng hoá
   Chạy bằng SSMS / Azure Data Studio / sqlcmd.
   Script được viết idempotent: chạy lại nhiều lần không gây lỗi.
   ===================================================================== */

IF DB_ID(N'QuanLyHangHoa') IS NULL
BEGIN
    CREATE DATABASE QuanLyHangHoa;
END
GO

USE QuanLyHangHoa;
GO

/* ---------------------------------------------------------------------
   Bảng LoaiSP - danh mục loại sản phẩm
   --------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.LoaiSP', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LoaiSP
    (
        id       INT IDENTITY(1,1) NOT NULL,
        TenLoai  NVARCHAR(100)     NOT NULL,
        CONSTRAINT PK_LoaiSP PRIMARY KEY (id),
        CONSTRAINT UQ_LoaiSP_TenLoai UNIQUE (TenLoai)
    );
END
GO

/* ---------------------------------------------------------------------
   Bảng SanPham - sản phẩm, khoá ngoại IdLoai -> LoaiSP(id)
   --------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.SanPham', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SanPham
    (
        id       INT IDENTITY(1,1) NOT NULL,
        IdLoai   INT               NOT NULL,
        TenSp    NVARCHAR(200)     NOT NULL,
        Gia      DECIMAL(18,2)     NOT NULL CONSTRAINT DF_SanPham_Gia DEFAULT (0),
        SoLuong  INT               NOT NULL CONSTRAINT DF_SanPham_SoLuong DEFAULT (0),
        HinhAnh  NVARCHAR(260)     NULL,
        NhaSX    NVARCHAR(150)     NULL,
        CONSTRAINT PK_SanPham PRIMARY KEY (id),
        CONSTRAINT FK_SanPham_LoaiSP FOREIGN KEY (IdLoai)
            REFERENCES dbo.LoaiSP(id),
        CONSTRAINT CK_SanPham_Gia CHECK (Gia >= 0),
        CONSTRAINT CK_SanPham_SoLuong CHECK (SoLuong >= 0)
    );

    CREATE INDEX IX_SanPham_IdLoai ON dbo.SanPham(IdLoai);
    CREATE INDEX IX_SanPham_TenSp  ON dbo.SanPham(TenSp);
END
GO

/* ---------------------------------------------------------------------
   Dữ liệu mẫu (chỉ nạp khi bảng còn trống)
   --------------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.LoaiSP)
BEGIN
    INSERT INTO dbo.LoaiSP (TenLoai) VALUES
        (N'Điện thoại'),
        (N'Laptop'),
        (N'Máy tính bảng'),
        (N'Phụ kiện'),
        (N'Đồng hồ thông minh');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SanPham)
BEGIN
    DECLARE @dienThoai INT = (SELECT id FROM dbo.LoaiSP WHERE TenLoai = N'Điện thoại');
    DECLARE @laptop    INT = (SELECT id FROM dbo.LoaiSP WHERE TenLoai = N'Laptop');
    DECLARE @tablet    INT = (SELECT id FROM dbo.LoaiSP WHERE TenLoai = N'Máy tính bảng');
    DECLARE @phuKien   INT = (SELECT id FROM dbo.LoaiSP WHERE TenLoai = N'Phụ kiện');
    DECLARE @dongHo    INT = (SELECT id FROM dbo.LoaiSP WHERE TenLoai = N'Đồng hồ thông minh');

    INSERT INTO dbo.SanPham (IdLoai, TenSp, Gia, SoLuong, HinhAnh, NhaSX) VALUES
        (@dienThoai, N'iPhone 15 Pro Max 256GB', 31990000, 12, NULL, N'Apple'),
        (@dienThoai, N'Samsung Galaxy S24 Ultra', 29990000, 8,  NULL, N'Samsung'),
        (@dienThoai, N'Xiaomi 14T Pro',          14990000, 25, NULL, N'Xiaomi'),
        (@laptop,    N'MacBook Air M3 13"',      27990000, 6,  NULL, N'Apple'),
        (@laptop,    N'Dell XPS 13 9340',        38990000, 4,  NULL, N'Dell'),
        (@laptop,    N'Asus TUF Gaming F15',     22490000, 10, NULL, N'Asus'),
        (@tablet,    N'iPad Air 11" M2',         16990000, 9,  NULL, N'Apple'),
        (@tablet,    N'Samsung Galaxy Tab S9',   19990000, 5,  NULL, N'Samsung'),
        (@phuKien,   N'Tai nghe AirPods Pro 2',   5490000, 40, NULL, N'Apple'),
        (@phuKien,   N'Chuột Logitech MX Master 3S', 2390000, 33, NULL, N'Logitech'),
        (@phuKien,   N'Sạc dự phòng Anker 20.000mAh', 990000, 57, NULL, N'Anker'),
        (@dongHo,    N'Apple Watch Series 9 45mm', 10990000, 7, NULL, N'Apple'),
        (@dongHo,    N'Garmin Forerunner 265',     11490000, 3, NULL, N'Garmin');
END
GO

/* ---------------------------------------------------------------------
   Kiểm tra nhanh
   --------------------------------------------------------------------- */
SELECT sp.id, l.TenLoai, sp.TenSp, sp.Gia, sp.SoLuong, sp.NhaSX
FROM dbo.SanPham sp
INNER JOIN dbo.LoaiSP l ON l.id = sp.IdLoai
ORDER BY sp.id;
GO
