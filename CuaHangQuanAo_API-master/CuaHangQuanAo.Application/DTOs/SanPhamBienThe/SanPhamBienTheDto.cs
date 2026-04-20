using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.SanPhamBienThe
{
    public class SanPhamBienTheDto
    {
        public int Id { get; set; }
        public int SanPhamId { get; set; }
        public int MauId { get; set; }
        public int SizeId { get; set; }
        public int SoLuongTon { get; set; }

        // kết nối TenMau, TenSize
        public string? TenMau { get; set; }
        public string? MaMau { get; set; }
        public string? TenSize { get; set; }
    }
}
