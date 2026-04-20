using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.MauSac
{
    public class MauSacDto
    {
        public int Id { get; set; }
        public string? TenMau { get; set; }
        public string? MaMau { get; set; }
        public int LoaiId { get; set; }
        public string? TenLoai { get; set; }
    }
}
