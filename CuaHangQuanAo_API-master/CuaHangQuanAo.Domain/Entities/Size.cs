using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Domain.Entities
{
    public class Size
    {
        public int Id { get; set; }
        public string? TenSize { get; set; }

        public int? LoaiId { get; set; }
        [ForeignKey("LoaiId")] 
        public virtual LoaiSP? LoaiSP { get; set; }
    }
}
