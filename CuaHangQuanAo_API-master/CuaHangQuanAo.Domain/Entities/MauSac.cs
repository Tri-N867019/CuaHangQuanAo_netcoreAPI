using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Domain.Entities
{
    public class MauSac
    {
        public int Id { get; set; }
        public string? MaMau { get; set; }
        public string? TenMau { get; set; }

        public int? LoaiId { get; set; }

        [ForeignKey("LoaiId")]
        public virtual LoaiSP? LoaiSP { get; set; }
    }
}
