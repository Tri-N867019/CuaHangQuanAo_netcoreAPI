using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.SanPham
{
    public class BienTheChiTietDto
    {
        public int Id { get; set; }
        public int? MauId { get; set; }
        public string? TenMau { get; set; }
        public string? MaMau { get; set; }
        public int? SizeId { get; set; }
        public string? TenSize { get; set; }
        public int? SoLuongTon { get; set; }
    }

    //  Chứa thông tin gốc và kéo theo cả mớ dữ liệu con
    public class SanPhamDetailDto
    {
        public int Id { get; set; }
        public string? Ten { get; set; }
        public string? MoTa { get; set; }
        public decimal? GiaBan { get; set; }
        public decimal? KhuyenMai { get; set; }
        public string? HuongDan { get; set; }
        public string? ThanhPhan { get; set; }

        // Tên Loại & Thương hiệu đã JOIN
        public string? TenLoai { get; set; }
        public string? TenThuongHieu { get; set; }

        // Danh sách hình ảnh (Chỉ cần mảng chuỗi chứa Link ảnh)
        public List<string> DanhSachAnh { get; set; } = new List<string>();

        // Danh sách các tùy chọn Màu/Size cho khách chọn
        public List<BienTheChiTietDto> CacBienThe { get; set; } = new List<BienTheChiTietDto>();
    }
}
