using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.GioHang
{
    public class GioHangDto
    {
        public int Id { get; set; }
        public int SanPhamBienTheId { get; set; }

        public string? TenSanPham { get; set; }
        public string? TenMau { get; set; }
        public string? TenSize { get; set; }
        public decimal GiaBan { get; set; }

        public int SoLuong { get; set; }
        public decimal ThanhTien => GiaBan * SoLuong; 
    }
}
