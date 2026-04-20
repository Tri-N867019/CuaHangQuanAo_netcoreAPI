using CuaHangQuanAo.Application.DTOs.GioHang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Interfaces
{
    public interface IGioHangService 
    {
        Task<IEnumerable<GioHangDto>> GetCartByUserIdAsync(int userId);
        Task<bool> AddToCartAsync(int userId, ThemGioHangDto dto);
        Task<bool> RemoveFromCartAsync(int userId, int cartId);
    }
}
