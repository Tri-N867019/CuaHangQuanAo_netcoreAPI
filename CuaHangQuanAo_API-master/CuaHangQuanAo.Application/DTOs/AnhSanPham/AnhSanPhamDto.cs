using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.AnhSanPham
{
    public class AnhSanPhamDto
    {
        public int Id { get; set; }
        public int SanPhamId { get; set; }
        public string? TenAnhSP { get; set; }
        public IFormFile? FileAnh { get; set; }
    }
}
