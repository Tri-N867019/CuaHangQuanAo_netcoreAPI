using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Domain.Entities
{
    public class SanPham
    {
        public int Id { get; set; }
        public string? Ten { get; set; }
        public string? MoTa { get; set; }
        public decimal? GiaBan { get; set; }
        public decimal? GiaNhap { get; set; }
        public decimal? KhuyenMai { get; set; }
        public string? HuongDan { get; set; }
        public string? ThanhPhan { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public string? TrangThaiSanPham { get; set; }
        public bool? TrangThaiHoatDong { get; set; }
        public byte? GioiTinh { get; set; }

        public int? ThuongHieuId { get; set; }
        public virtual ThuongHieu? ThuongHieu { get; set; } 

        public int? LoaiId { get; set; }
        public virtual LoaiSP? LoaiSP { get; set; }

        public virtual ICollection<AnhSanPham>? AnhSanPhams { get; set; }
        public virtual ICollection<SanPhamBienThe>? SanPhamBienThes { get; set; }
    }
}
