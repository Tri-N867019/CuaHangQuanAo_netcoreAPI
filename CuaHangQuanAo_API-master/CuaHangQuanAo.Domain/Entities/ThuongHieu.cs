using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Domain.Entities
{
    public class ThuongHieu
    {
        public int Id { get; set; }
        public string? TenTH { get; set; }
        public string? MoTa { get; set; }
        public DateTime? NgayTao { get; set; }
    }
}
