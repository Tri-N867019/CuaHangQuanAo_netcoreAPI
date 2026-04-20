using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.SanPhamBienThe
{
    public class CreateUpdateSanPhamBienTheDto
    {
        public int SanPhamId { get; set; }
        public int MauId { get; set; }
        public int SizeId { get; set; }
        public int SoLuongTon { get; set; }
    }
}
