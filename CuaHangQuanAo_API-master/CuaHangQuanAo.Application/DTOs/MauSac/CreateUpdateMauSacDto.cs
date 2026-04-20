using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.MauSac
{
    public class CreateUpdateMauSacDto
    {
        public string? TenMau { get; set; }
        public string? MaMau { get; set; } 
        public int LoaiId { get; set; }
    }
}
