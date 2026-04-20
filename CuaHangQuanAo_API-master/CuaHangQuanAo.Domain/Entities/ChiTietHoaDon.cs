using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Domain.Entities
{
    public class ChiTietHoaDon
    {
        public int Id { get; set; }
        public int? HoaDonId { get; set; }
        public virtual HoaDon? HoaDon { get; set; }
        public int? SanPhamBienTheId { get; set; }
        public virtual SanPhamBienThe? SanPhamBienThe { get; set; }
        public string? TenSanPham { get; set; } 
        public string? TenMau { get; set; }     
        public string? TenSize { get; set; }   
        public int? Soluong { get; set; }
        public decimal? GiaBan { get; set; }    
        public decimal? ThanhTien { get; set; }
    }
}
