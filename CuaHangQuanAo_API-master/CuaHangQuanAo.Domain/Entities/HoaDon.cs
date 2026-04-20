using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Domain.Entities
{
    public class HoaDon
    {
        public int Id { get; set; }
        public DateTime? NgayTao { get; set; }
        public decimal? TongTien { get; set; }
        public decimal? PhiVanChuyen { get; set; }      
        public string? TenNguoiNhan { get; set; }
        public string? SDTNhanHang { get; set; }
        public string? DiaChiGiaoHang { get; set; }
        public byte? TrangThai { get; set; }           
        public string? PhuongThucThanhToan { get; set; } 
        public byte? TrangThaiThanhToan { get; set; }
        public string? GhiChu { get; set; }

        public int? UserId { get; set; }
        public virtual Users? User { get; set; }
        public virtual ICollection<ChiTietHoaDon>? ChiTietHoaDons { get; set; }
    }
}
