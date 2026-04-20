using CuaHangQuanAo.Application.Interfaces;
using CuaHangQuanAo.Domain.Entities;
using CuaHangQuanAo.Domain.Interfaces;
using CuaHangQuanAo.Application.DTOs.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<Users> _repository;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public UserService(IGenericRepository<Users> repository, IConfiguration config, IEmailService emailService)
        {
            _repository = repository;
            _config = config;
            _emailService = emailService;
        }

        // 1. Đăng kí tài khoản 
        public async Task AddAsync(CreateUserDto dto)
        {
            var users = await _repository.GetAllAsync();
            if (users.Any(u => u.TenDangNhap == dto.TenDangNhap))
                throw new Exception("Tên đăng nhập này đã tồn tại trong hệ thống!");
            
            if (users.Any(u => u.Email == dto.Email))
                throw new Exception("Email này đã được sử dụng cho tài khoản khác!");

            var user = new Users
            {
                TenDangNhap = dto.TenDangNhap,
                Email = dto.Email,
                HoVaTen = dto.HoVaTen,
                PhanQuyenId = (dto.PhanQuyenId <= 0 || dto.PhanQuyenId > 4) ? 4 : dto.PhanQuyenId,
                // Mã hóa mật khẩu bằng BCrypt
                MatKhau = BCrypt.Net.BCrypt.HashPassword(dto.MatKhau)
            };
            await _repository.AddAsync(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _repository.GetAllAsync(x => x.PhanQuyen!);
            return users.Select(user => new UserDto
            {
                Id = user.Id,
                TenDangNhap = user.TenDangNhap,
                Email = user.Email,
                SDT = user.SDT,
                HoVaTen = user.HoVaTen,
                DiaChi = user.DiaChi,
                AnhDaiDien = user.AnhDaiDien,
                PhanQuyenId = user.PhanQuyenId ?? 0,
                TenQuyen = user.PhanQuyen?.TenQuyen
            });
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var users = await _repository.GetAllAsync(x => x.PhanQuyen!);
            var user = users.FirstOrDefault(x => x.Id == id);

            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                TenDangNhap = user.TenDangNhap,
                Email = user.Email,
                SDT = user.SDT,
                HoVaTen = user.HoVaTen,
                DiaChi = user.DiaChi,
                AnhDaiDien = user.AnhDaiDien,
                PhanQuyenId = user.PhanQuyenId ?? 0,
                TenQuyen = user.PhanQuyen?.TenQuyen
            };
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var users = await _repository.GetAllAsync(x => x.PhanQuyen!);
            var user = users.FirstOrDefault(x => x.TenDangNhap == dto.TenDangNhap);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.MatKhau, user.MatKhau))
            {
                return null;
            }

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(Users user)
        {
            // ĐỌC CHUẨN TỪ APPSETTINGS.JSON
            var secretKey = _config["JwtSettings:SecretKey"];
            var issuer = _config["JwtSettings:Issuer"];
            var audience = _config["JwtSettings:Audience"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.TenDangNhap ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("role", user.PhanQuyen?.TenQuyen.Trim() ?? "khachhang"),
                new Claim(ClaimTypes.Role, user.PhanQuyen?.TenQuyen.Trim() ?? "khachhang")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,    
                audience: audience, 
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> CapQuyenAsync(int callerId, int userId, int quyenMoiId)
        {
            var caller = await _repository.GetByIdAsync(callerId);
            var target = await _repository.GetByIdAsync(userId);
            
            if (caller == null || target == null) return false;

            // Lấy thông tin quyền (Giả sử ID 1 là Admin1)
            bool isSuperAdmin = (caller.PhanQuyenId == 1 || string.Equals(caller.TenDangNhap, "admin1", StringComparison.OrdinalIgnoreCase));
            bool isAdmin = (caller.PhanQuyenId == 2);

            // 1. Nếu Target là Admin1 -> Chỉ Admin1 mới được sửa chính mình (hoặc Admin1 khác nếu có)
            if (target.PhanQuyenId == 1 || string.Equals(target.TenDangNhap, "admin1", StringComparison.OrdinalIgnoreCase))
            {
                if (!isSuperAdmin) 
                    throw new UnauthorizedAccessException("Bạn không có quyền thay đổi thông tin của Chủ trang web (Admin1)!");
            }

            // 2. Nếu Người thực hiện là Admin thường
            if (isAdmin)
            {
                // Quy tắc: Không được hạ quyền (ID quyền mới không được lớn hơn ID quyền hiện tại)
                // Trong hệ thống này: 1 (Admin1) < 2 (Admin) < 3 (Nhân viên) < 4 (Khách hàng)
                if (quyenMoiId > (target.PhanQuyenId ?? 4))
                {
                    throw new UnauthorizedAccessException("Cấp Quản trị viên (Admin) không có quyền hạ cấp bậc của người dùng khác!");
                }
            }

            target.PhanQuyenId = quyenMoiId;
            await _repository.UpdateAsync(target);

            return true;
        }

        public async Task<bool> UpdateProfileAsync(int userId, UpdateUserDto dto)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null) return false;

            if (!string.IsNullOrEmpty(dto.HoVaTen)) user.HoVaTen = dto.HoVaTen;
            if (!string.IsNullOrEmpty(dto.SDT)) user.SDT = dto.SDT;
            if (!string.IsNullOrEmpty(dto.DiaChi)) user.DiaChi = dto.DiaChi;

            await _repository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> UpdateAvatarAsync(int userId, string avatarUrl)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null) return false;

            user.AnhDaiDien = avatarUrl;
            await _repository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null) return false;

            // Kiểm tra mật khẩu cũ
            if (!BCrypt.Net.BCrypt.Verify(dto.MatKhauCu, user.MatKhau))
            {
                throw new Exception("Mật khẩu cũ không chính xác!");
            }

            // Hash mật khẩu mới và lưu
            user.MatKhau = BCrypt.Net.BCrypt.HashPassword(dto.MatKhauMoi);
            await _repository.UpdateAsync(user);

            return true;
        }
        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var users = await _repository.GetAllAsync();
            var user = users.FirstOrDefault(x => x.Email == email);

            if (user == null) return false;

            string newPassword = Guid.NewGuid().ToString("N").Substring(0, 8); // Tạo chuỗi ngẫu nhiên 8 ký tự

            user.MatKhau = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _repository.UpdateAsync(user);

            string subject = "Reset Mật khẩu - NT Clothing";
            string body = $@"
                <h3>Chào {user.HoVaTen},</h3>
                <p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
                <p>Mật khẩu mới của bạn là: <strong>{newPassword}</strong></p>
                <p>Vui lòng đăng nhập và đổi mật khẩu ngay để đảm bảo an toàn.</p>
                <br/>
                <p>Trân trọng,<br/>Đội ngũ NT Clothing</p>";

            await _emailService.SendEmailAsync(email, subject, body);
            
            return true;
        }
    }
}