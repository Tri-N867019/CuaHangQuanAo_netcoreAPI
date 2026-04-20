using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.SanPham
{
    public class SanPhamDto
    {
        public int Id { get; set; }
        public string? Ten { get; set; }
        public string? MoTa { get; set; }
        public decimal? GiaBan { get; set; }

        public int LoaiId { get; set; }
        public int ThuongHieuId { get; set; }
        public byte? GioiTinh { get; set; }

        public decimal? GiaNhap { get; set; }
        public string? TrangThaiSanPham { get; set; }
        public bool? TrangThaiHoatDong { get; set; }
        public int SoLuong { get; set; }

        public string? TenLoai { get; set; }
        public string? TenThuongHieu { get; set; }
        public string? HinhAnh { get; set; }
        public decimal? KhuyenMai { get; set; }
        public List<int>? MauIds { get; set; }
        public List<string>? MaMaus { get; set; }
    }
}
