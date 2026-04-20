using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? TenDangNhap { get; set; }
        public string? Email { get; set; }
        public string? SDT { get; set; }
        public string? HoVaTen { get; set; }
        public string? DiaChi { get; set; }
        public string? AnhDaiDien { get; set; }
        public int PhanQuyenId { get; set; }
        public string? TenQuyen { get; set; }
    }
}
