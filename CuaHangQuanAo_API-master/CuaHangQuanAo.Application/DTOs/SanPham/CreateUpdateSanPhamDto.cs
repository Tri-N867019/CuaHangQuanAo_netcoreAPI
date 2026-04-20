using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.SanPham
{
    public class CreateUpdateSanPhamDto
    {
        public string? Ten { get; set; }
        public string? MoTa { get; set; }
        public decimal? GiaBan { get; set; }
        public decimal? GiaNhap { get; set; }
        public decimal? KhuyenMai { get; set; }
        public string? HuongDan { get; set; }
        public string? ThanhPhan { get; set; }
        public string? TrangThaiSanPham { get; set; }
        public bool? TrangThaiHoatDong { get; set; }
        public byte? GioiTinh { get; set; }

        public int LoaiId { get; set; }
        public int ThuongHieuId { get; set; }
    }
}
