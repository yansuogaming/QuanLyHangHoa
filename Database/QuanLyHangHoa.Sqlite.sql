-- =====================================================================
-- QuanLyHangHoa - phiên bản SQLite (dùng khi máy không cài SQL Server)
-- Cấu trúc bảng giữ đúng như bản SQL Server trong QuanLyHangHoa.sql.
-- File CSDL: QuanLyHangHoa.db nằm cạnh QuanLySP.exe
-- =====================================================================

PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS LoaiSP
(
    id      INTEGER       NOT NULL PRIMARY KEY AUTOINCREMENT,
    TenLoai NVARCHAR(100) NOT NULL COLLATE NOCASE UNIQUE
);

CREATE TABLE IF NOT EXISTS SanPham
(
    id      INTEGER        NOT NULL PRIMARY KEY AUTOINCREMENT,
    IdLoai  INTEGER        NOT NULL REFERENCES LoaiSP(id),
    TenSp   NVARCHAR(200)  NOT NULL,
    Gia     DECIMAL(18,2)  NOT NULL DEFAULT 0 CHECK (Gia >= 0),
    SoLuong INTEGER        NOT NULL DEFAULT 0 CHECK (SoLuong >= 0),
    HinhAnh NVARCHAR(260)  NULL,
    NhaSX   NVARCHAR(150)  NULL
);

CREATE INDEX IF NOT EXISTS IX_SanPham_IdLoai ON SanPham(IdLoai);
CREATE INDEX IF NOT EXISTS IX_SanPham_TenSp  ON SanPham(TenSp);

-- @SEED --------------------------------------------------------------
-- Phần dưới chỉ chạy khi bảng SanPham còn trống (xem DatabaseInitializer).

INSERT OR IGNORE INTO LoaiSP (TenLoai) VALUES
    ('Điện thoại'),
    ('Laptop'),
    ('Máy tính bảng'),
    ('Phụ kiện'),
    ('Đồng hồ thông minh');

INSERT INTO SanPham (IdLoai, TenSp, Gia, SoLuong, HinhAnh, NhaSX)
SELECT l.id, d.TenSp, d.Gia, d.SoLuong, NULL, d.NhaSX
FROM (
    SELECT 'Điện thoại'        AS TenLoai, 'iPhone 15 Pro Max 256GB'      AS TenSp, 31990000 AS Gia, 12 AS SoLuong, 'Apple'    AS NhaSX
    UNION ALL SELECT 'Điện thoại',        'Samsung Galaxy S24 Ultra',     29990000,  8, 'Samsung'
    UNION ALL SELECT 'Điện thoại',        'Xiaomi 14T Pro',               14990000, 25, 'Xiaomi'
    UNION ALL SELECT 'Laptop',            'MacBook Air M3 13"',           27990000,  6, 'Apple'
    UNION ALL SELECT 'Laptop',            'Dell XPS 13 9340',             38990000,  4, 'Dell'
    UNION ALL SELECT 'Laptop',            'Asus TUF Gaming F15',          22490000, 10, 'Asus'
    UNION ALL SELECT 'Máy tính bảng',     'iPad Air 11" M2',              16990000,  9, 'Apple'
    UNION ALL SELECT 'Máy tính bảng',     'Samsung Galaxy Tab S9',        19990000,  5, 'Samsung'
    UNION ALL SELECT 'Phụ kiện',          'Tai nghe AirPods Pro 2',        5490000, 40, 'Apple'
    UNION ALL SELECT 'Phụ kiện',          'Chuột Logitech MX Master 3S',   2390000, 33, 'Logitech'
    UNION ALL SELECT 'Phụ kiện',          'Sạc dự phòng Anker 20.000mAh',   990000, 57, 'Anker'
    UNION ALL SELECT 'Đồng hồ thông minh','Apple Watch Series 9 45mm',    10990000,  7, 'Apple'
    UNION ALL SELECT 'Đồng hồ thông minh','Garmin Forerunner 265',        11490000,  3, 'Garmin'
) AS d
INNER JOIN LoaiSP l ON l.TenLoai = d.TenLoai;
