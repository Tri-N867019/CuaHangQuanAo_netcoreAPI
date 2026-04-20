using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.Size
{
    public class CreateUpdateSizeDto
    {
        public string? TenSize { get; set; } 
        public int LoaiId { get; set; }
    }
}
