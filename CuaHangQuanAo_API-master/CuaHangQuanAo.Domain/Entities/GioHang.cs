using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Domain.Entities
{
    public class GioHang
    {
        public int Id { get; set; }
        public int? SoLuong { get; set; }
        public DateTime NgayThem { get; set; } = DateTime.Now;
        public int? UserId { get; set; }
        public virtual Users? User { get; set; }

        public int? SanPhamBienTheId { get; set; }
        public virtual SanPhamBienThe? SanPhamBienThe { get; set; }
    }
}
