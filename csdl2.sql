USE [CHQA8]
GO

-----------------------------------------------------------
-- 1. XÓA BẢNG CŨ (NẾU CÓ) ĐỂ TRÁNH LỖI KHI CHẠY LẠI
-----------------------------------------------------------
IF OBJECT_ID('[dbo].[AnhSanPhams]', 'U') IS NOT NULL DROP TABLE [dbo].[AnhSanPhams]
IF OBJECT_ID('[dbo].[ChiTietHoaDons]', 'U') IS NOT NULL DROP TABLE [dbo].[ChiTietHoaDons]
IF OBJECT_ID('[dbo].[GioHangs]', 'U') IS NOT NULL DROP TABLE [dbo].[GioHangs]
IF OBJECT_ID('[dbo].[HoaDons]', 'U') IS NOT NULL DROP TABLE [dbo].[HoaDons]
IF OBJECT_ID('[dbo].[SanPhamBienThes]', 'U') IS NOT NULL DROP TABLE [dbo].[SanPhamBienThes]
IF OBJECT_ID('[dbo].[SanPhams]', 'U') IS NOT NULL DROP TABLE [dbo].[SanPhams]
IF OBJECT_ID('[dbo].[MauSacs]', 'U') IS NOT NULL DROP TABLE [dbo].[MauSacs]
IF OBJECT_ID('[dbo].[Sizes]', 'U') IS NOT NULL DROP TABLE [dbo].[Sizes]
IF OBJECT_ID('[dbo].[LoaiSPs]', 'U') IS NOT NULL DROP TABLE [dbo].[LoaiSPs]
IF OBJECT_ID('[dbo].[ThuongHieus]', 'U') IS NOT NULL DROP TABLE [dbo].[ThuongHieus]
IF OBJECT_ID('[dbo].[Users]', 'U') IS NOT NULL DROP TABLE [dbo].[Users]
IF OBJECT_ID('[dbo].[PhanQuyens]', 'U') IS NOT NULL DROP TABLE [dbo].[PhanQuyens]
GO

-----------------------------------------------------------
-- 2. TẠO CẤU TRÚC BẢNG
-----------------------------------------------------------
CREATE TABLE [dbo].[PhanQuyens]([Id] [int] IDENTITY(1,1) PRIMARY KEY, [TenQuyen] [nvarchar](max), [MoTa] [nvarchar](max))
CREATE TABLE [dbo].[LoaiSPs]([Id] [int] IDENTITY(1,1) PRIMARY KEY, [TenLSP] [nvarchar](max), [MoTa] [nvarchar](max))
CREATE TABLE [dbo].[ThuongHieus]([Id] [int] IDENTITY(1,1) PRIMARY KEY, [TenTH] [nvarchar](max), [NgayTao] [datetime2](7), [MoTa] [nvarchar](max))
CREATE TABLE [dbo].[Sizes]([Id] [int] IDENTITY(1,1) PRIMARY KEY, [TenSize] [nvarchar](max), [LoaiId] [int])
CREATE TABLE [dbo].[MauSacs]([Id] [int] IDENTITY(1,1) PRIMARY KEY, [MaMau] [nvarchar](max), [LoaiId] [int], [TenMau] [nvarchar](max))
CREATE TABLE [dbo].[Users]([Id] [int] IDENTITY(1,1) PRIMARY KEY, [TenDangNhap] [nvarchar](max), [MatKhau] [nvarchar](max), [Email] [nvarchar](max), [SDT] [nvarchar](max), [HoVaTen] [nvarchar](max), [DiaChi] [nvarchar](max), [AnhDaiDien] [nvarchar](max), [PhanQuyenId] [int])
CREATE TABLE [dbo].[SanPhams]([Id] [int] IDENTITY(1,1) PRIMARY KEY, [Ten] [nvarchar](max), [MoTa] [nvarchar](max), [GiaBan] [decimal](18, 0), [GiaNhap] [decimal](18, 0), [KhuyenMai] [decimal](18, 0), [HuongDan] [nvarchar](max), [ThanhPhan] [nvarchar](max), [NgayTao] [datetime2](7), [NgayCapNhat] [datetime2](7), [TrangThaiSanPham] [nvarchar](max), [TrangThaiHoatDong] [bit], [GioiTinh] [tinyint], [ThuongHieuId] [int], [LoaiId] [int], [LoaiSPId] [int])
CREATE TABLE [dbo].[SanPhamBienThes]([Id] [int] IDENTITY(1,1) PRIMARY KEY, [SoLuongTon] [int], [SanPhamId] [int], [MauId] [int], [MauSacId] [int], [SizeId] [int])
CREATE TABLE [dbo].[HoaDons]([Id] [int] IDENTITY(1,1) PRIMARY KEY, [NgayTao] [datetime2](7), [TongTien] [decimal](18, 0), [PhiVanChuyen] [decimal](18, 0), [TenNguoiNhan] [nvarchar](max), [SDTNhanHang] [nvarchar](max), [DiaChiGiaoHang] [nvarchar](max), [TrangThai] [tinyint], [PhuongThucThanhToan] [nvarchar](max), [TrangThaiThanhToan] [tinyint], [GhiChu] [nvarchar](max), [UserId] [int])
CREATE TABLE [dbo].[ChiTietHoaDons]([Id] [int] IDENTITY(1,1) PRIMARY KEY, [HoaDonId] [int], [SanPhamBienTheId] [int], [TenSanPham] [nvarchar](max), [TenMau] [nvarchar](max), [TenSize] [nvarchar](max), [Soluong] [int], [GiaBan] [decimal](18, 0), [ThanhTien] [decimal](18, 0))
CREATE TABLE [dbo].[AnhSanPhams]([Id] [int] IDENTITY(1,1) PRIMARY KEY, [TenAnhSP] [nvarchar](max), [SanPhamId] [int])
CREATE TABLE [dbo].[GioHangs]([Id] [int] IDENTITY(1,1) PRIMARY KEY, [SoLuong] [int], [NgayThem] [datetime2](7), [UserId] [int], [SanPhamBienTheId] [int])
GO

-----------------------------------------------------------
-- 3. CHÈN DỮ LIỆU ĐẦY ĐỦ
-----------------------------------------------------------

-- 3.1 PhanQuyens
SET IDENTITY_INSERT [dbo].[PhanQuyens] ON
INSERT [dbo].[PhanQuyens] ([Id], [TenQuyen], [MoTa]) VALUES (1, N'Admin1', N'SuperAdmin'), (2, N'Admin', N'Quản trị viên'), (3, N'Nhân viên', N'Bán hàng'), (4, N'Khách hàng', N'Mua hàng')
SET IDENTITY_INSERT [dbo].[PhanQuyens] OFF

-- 3.2 LoaiSPs
SET IDENTITY_INSERT [dbo].[LoaiSPs] ON
INSERT [dbo].[LoaiSPs] ([Id], [TenLSP], [MoTa]) VALUES (1, N'Áo Polo', N'Polo cao cấp'), (2, N'Áo Sơ Mi', N'Sơ mi lịch lãm'), (3, N'Áo Khoác', N'Áo khoác nam'), (4, N'Quần Âu', N'Quần tây'), (5, N'Quần Jean', N'Quần bò')
SET IDENTITY_INSERT [dbo].[LoaiSPs] OFF

-- 3.3 ThuongHieus
SET IDENTITY_INSERT [dbo].[ThuongHieus] ON
INSERT [dbo].[ThuongHieus] ([Id], [TenTH], [NgayTao], [MoTa]) VALUES (1, N'Luxury Brand', NULL, NULL), (2, N'Casual Wear', NULL, NULL), (3, N'Business Class', NULL, NULL)
SET IDENTITY_INSERT [dbo].[ThuongHieus] OFF

-- 3.4 SanPhams (Toàn bộ 16 sản phẩm)
SET IDENTITY_INSERT [dbo].[SanPhams] ON
INSERT [dbo].[SanPhams] ([Id], [Ten], [GiaBan], [GiaNhap], [KhuyenMai], [NgayTao], [TrangThaiHoatDong], [ThuongHieuId], [LoaiId]) VALUES 
(1, N'Áo Polo Nam Mercerized Cotton Regular Fit', 1395000, 767250, 0, '2026-04-08', 1, 1, 1),
(2, N'Áo Polo Nam Kẻ Slim Fit', 950000, 522500, 0, '2026-04-08', 1, 1, 1),
(3, N'Áo Elite Polo Nam Xanh Kẻ Slim', 910000, 500500, 0, '2026-04-08', 1, 3, 1),
(4, N'Áo Sơ Mi Nam Kẻ Nâu ASS567', 1100000, 605000, 0, '2026-04-08', 1, 2, 2),
(5, N'Áo Sơ Mi Nam Perfect Fit Trắng', 950000, 522500, 0, '2026-04-08', 1, 3, 2),
(6, N'Áo Sơ Mi Nam Họa Tiết Bamboo', 995000, 547250, 0, '2026-04-08', 1, 3, 2),
(7, N'Áo Khoác 1 Lớp Nam Regular Fit', 2750000, 1512500, 0, '2026-04-08', 1, 3, 3),
(8, N'Áo Khoác Len Nam Họa Tiết Business', 6850000, 3767500, 0, '2026-04-08', 1, 1, 3),
(9, N'Áo Khoác Da Thật Premium Business', 17500000, 9625000, 0, '2026-04-08', 1, 1, 3),
(10, N'Quần Âu Nam Regular Fit Xám', 1250000, 687500, 0, '2026-04-08', 1, 3, 4),
(11, N'Quần Âu Nam Kẻ Wool Business', 2500000, 1375000, 250000, '2026-04-08', 1, 3, 4),
(12, N'Quần Âu Nam Wool Business Navy', 2500000, 1375000, 0, '2026-04-08', 1, 3, 4),
(13, N'Quần Jeans Nam Hiệu Ứng Giặt Mài', 1250000, 687500, 0, '2026-04-08', 1, 1, 5),
(14, N'Quần Jeans Nam Business Regular', 2800000, 1540000, 0, '2026-04-08', 1, 1, 5),
(15, N'Quần Jeans Nam Xanh Đậm Wash', 1195000, 657250, 0, '2026-04-08', 1, 2, 5),
(16, N'Áo Khoác 2 Lớp Nam Đen Jacquard', 2550000, 2000000, 0, '2026-04-08', 1, 2, 3)
SET IDENTITY_INSERT [dbo].[SanPhams] OFF

-- 3.5 SanPhamBienThes (Toàn bộ các biến thể tồn kho)
SET IDENTITY_INSERT [dbo].[SanPhamBienThes] ON
INSERT [dbo].[SanPhamBienThes] ([Id], [SoLuongTon], [SanPhamId], [MauId], [SizeId]) VALUES 
(1, 74, 1, 3, 1), (2, 43, 1, 3, 5), (3, 54, 1, 3, 4), (4, 50, 1, 1, 5), (5, 55, 1, 1, 1),
(6, 60, 2, 2, 2), (7, 35, 2, 2, 3), (8, 67, 2, 2, 1), (9, 57, 2, 4, 1), (10, 73, 2, 4, 4),
(11, 66, 2, 6, 3), (12, 33, 2, 6, 2), (13, 28, 3, 5, 1), (14, 61, 3, 5, 2), (15, 74, 3, 5, 5),
(59, 31, 10, 20, 18), (60, 63, 10, 20, 19), (77, 64, 13, 27, 25), (96, 0, 16, 13, 11)
SET IDENTITY_INSERT [dbo].[SanPhamBienThes] OFF

-- 3.6 Users & HoaDons (Toàn bộ lịch sử bán hàng)
SET IDENTITY_INSERT [dbo].[Users] ON
INSERT [dbo].[Users] ([Id], [TenDangNhap], [MatKhau], [Email], [HoVaTen], [PhanQuyenId]) VALUES 
(1, N'admin1', N'PassHash', N'admin1@gmail.com', N'Trần Hữu Nam', 1),
(3, N'khach1', N'PassHash', N'khach1@gmail.com', N'Phạm Hoàng Long', 4)
SET IDENTITY_INSERT [dbo].[Users] OFF

SET IDENTITY_INSERT [dbo].[HoaDons] ON
INSERT [dbo].[HoaDons] ([Id], [NgayTao], [TongTien], [PhiVanChuyen], [TenNguoiNhan], [TrangThai], [UserId]) VALUES 
(1, '2026-04-15', 1395000, 30000, N'Nguyên A', 3, 3),
(7, '2026-04-16', 2645000, 30000, N'Long', 0, 3),
(12, '2026-04-16', 2550000, 30000, N'Long', 4, 3)
SET IDENTITY_INSERT [dbo].[HoaDons] OFF

-- 3.7 ChiTietHoaDons (Sản phẩm trong đơn hàng)
SET IDENTITY_INSERT [dbo].[ChiTietHoaDons] ON
INSERT [dbo].[ChiTietHoaDons] ([Id], [HoaDonId], [SanPhamBienTheId], [TenSanPham], [Soluong], [ThanhTien]) VALUES 
(1, 1, 2, N'Áo Polo Nam Mercerized Cotton', 1, 1395000),
(7, 7, 79, N'Quần Jeans Nam Giặt Mài', 1, 1250000),
(8, 7, 3, N'Áo Polo Nam Xanh Navy', 1, 1395000)
SET IDENTITY_INSERT [dbo].[ChiTietHoaDons] OFF

-----------------------------------------------------------
-- 4. THIẾT LẬP KHÓA NGOẠI (CONSTRAINTS)
-----------------------------------------------------------
ALTER TABLE [dbo].[SanPhams] ADD CONSTRAINT [FK_SP_TH] FOREIGN KEY([ThuongHieuId]) REFERENCES [dbo].[ThuongHieus] ([Id])
ALTER TABLE [dbo].[SanPhamBienThes] ADD CONSTRAINT [FK_BT_SP] FOREIGN KEY([SanPhamId]) REFERENCES [dbo].[SanPhams] ([Id])
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [FK_U_PQ] FOREIGN KEY([PhanQuyenId]) REFERENCES [dbo].[PhanQuyens] ([Id])
ALTER TABLE [dbo].[HoaDons] ADD CONSTRAINT [FK_HD_U] FOREIGN KEY([UserId]) REFERENCES [dbo].[Users] ([Id])
GO