using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Domain.Entities
{
    public class AnhSanPham
    {
        public int Id { get; set; }
        public string? TenAnhSP { get; set; }

        public int? SanPhamId { get; set; }
        public virtual SanPham? SanPham { get; set; }
    }
}
