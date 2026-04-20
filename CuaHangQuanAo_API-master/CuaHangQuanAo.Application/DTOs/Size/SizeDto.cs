using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.Size
{
    public class SizeDto
    {
        public int Id { get; set; }
        public string? TenSize { get; set; }
        public int LoaiId { get; set; }
        public string? TenLoai { get; set; }
    }
}
