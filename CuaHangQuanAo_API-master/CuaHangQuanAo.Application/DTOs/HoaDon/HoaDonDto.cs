using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.HoaDon
{
    public class HoaDonDto
    {
        public int Id { get; set; }
        public string? MaDonHang { get; set; }
        public DateTime? NgayTao { get; set; }
        public decimal? TongTien { get; set; }
        public decimal? PhiVanChuyen { get; set; }
        public string? TenNguoiNhan { get; set; }
        public string? SDTNhanHang { get; set; }
        public string? DiaChiGiaoHang { get; set; }
        public byte? TrangThai { get; set; }
        public string? PhuongThucThanhToan { get; set; }

        // Chứa danh sách các món đồ đã mua trong đơn này
        public List<ChiTietHoaDonDto> ChiTiet { get; set; } = new List<ChiTietHoaDonDto>();
    }
}
