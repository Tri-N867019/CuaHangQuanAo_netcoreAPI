using CuaHangQuanAo.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Interfaces
{
    public interface IUserService
    {
        Task AddAsync(CreateUserDto dto);
        Task<string?> LoginAsync(LoginDto dto);
        Task<UserDto?> GetByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<bool> CapQuyenAsync(int callerId, int userId, int quyenMoiId);
        Task<bool> UpdateProfileAsync(int userId, UpdateUserDto dto);
        Task<bool> UpdateAvatarAsync(int userId, string avatarUrl);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task<bool> ForgotPasswordAsync(string email);
    }
}
