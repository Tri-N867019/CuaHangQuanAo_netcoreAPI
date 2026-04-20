using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.HoaDon
{
    public class ChiTietHoaDonDto
    {
        public string? TenSanPham { get; set; }
        public string? TenMau { get; set; }
        public string? TenSize { get; set; }
        public int? Soluong { get; set; }
        public decimal? GiaBan { get; set; }
        public decimal? ThanhTien { get; set; }
    }
}
