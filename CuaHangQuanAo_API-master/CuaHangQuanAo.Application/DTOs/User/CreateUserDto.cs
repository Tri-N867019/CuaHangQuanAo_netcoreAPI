using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.User
{
    public class CreateUserDto
    {
        public string? TenDangNhap { get; set; }
        public string? MatKhau { get; set; } 
        public string? Email { get; set; }
        public string? HoVaTen { get; set; }
        public int PhanQuyenId { get; set; } = 4;
        
    }
}
