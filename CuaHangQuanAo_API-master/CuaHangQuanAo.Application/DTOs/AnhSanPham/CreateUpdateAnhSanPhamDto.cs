using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.AnhSanPham
{
    public class CreateUpdateAnhSanPhamDto
    {
        public int SanPhamId { get; set; }
        public string? TenAnhSP { get; set; }
    }
}
