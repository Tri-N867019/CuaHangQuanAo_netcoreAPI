using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.HoaDon
{
    public class DatHangDto
    {
        public List<int>? CartItemIds { get; set; }
        public string? TenNguoiNhan { get; set; }
        public string? SDTNhanHang { get; set; }
        public string? DiaChiGiaoHang { get; set; }
        public string? PhuongThucThanhToan { get; set; }
        public string? GhiChu { get; set; }

    }
}
