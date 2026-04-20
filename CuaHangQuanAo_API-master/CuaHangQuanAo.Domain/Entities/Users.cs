using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Domain.Entities
{
    public class Users
    {
        public int Id { get; set; }
        public string? TenDangNhap { get; set; }
        public string? MatKhau { get; set; }
        public string? Email { get; set; }
        public string? SDT { get; set; }
        public string? HoVaTen { get; set; }
        public string? DiaChi { get; set; }
        public string? AnhDaiDien { get; set; }
        public int? PhanQuyenId { get; set; } 
        public virtual PhanQuyen? PhanQuyen { get; set; } 
    }
}
