using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Domain.Entities
{
    public class SanPhamBienThe
    {
        public int Id { get; set; }
        public int? SoLuongTon { get; set; }

        public int? SanPhamId { get; set; }
        [ForeignKey("SanPhamId")]
        public virtual SanPham? SanPham { get; set; }

        public int? MauId { get; set; }
        [ForeignKey("MauId")]
        public virtual MauSac? MauSac { get; set; }

        public int? SizeId { get; set; }
        [ForeignKey("SizeId")]
        public virtual Size? Size { get; set; }
    }
}
