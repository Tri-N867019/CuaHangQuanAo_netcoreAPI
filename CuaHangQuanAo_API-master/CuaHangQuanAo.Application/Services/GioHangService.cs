using CuaHangQuanAo.Application.DTOs.GioHang;
using CuaHangQuanAo.Application.Interfaces;
using CuaHangQuanAo.Domain.Entities;
using CuaHangQuanAo.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Services
{
    public class GioHangService : IGioHangService
    {
        private readonly IGenericRepository<GioHang> _gioHangRepo;
        private readonly IGenericRepository<SanPhamBienThe> _bienTheRepo;

        public GioHangService(IGenericRepository<GioHang> gioHangRepo, IGenericRepository<SanPhamBienThe> bienTheRepo)
        {
            _gioHangRepo = gioHangRepo;
            _bienTheRepo = bienTheRepo;
        }

        public async Task<IEnumerable<GioHangDto>> GetCartByUserIdAsync(int userId)
        {
            // Lấy giỏ hàng của user đó, Include thêm biến thể, sản phẩm, màu, size để lấy tên hiển thị
            var data = await _gioHangRepo.GetAllAsync(
                x => x.SanPhamBienThe!,
                x => x.SanPhamBienThe!.SanPham!,
                x => x.SanPhamBienThe!.MauSac!,
                x => x.SanPhamBienThe!.Size!
            );

            var myCart = data.Where(x => x.UserId == userId);

            return myCart.Select(x => new GioHangDto
            {
                Id = x.Id,
                SanPhamBienTheId = x.SanPhamBienTheId ?? 0,
                TenSanPham = x.SanPhamBienThe?.SanPham?.Ten,
                TenMau = x.SanPhamBienThe?.MauSac?.TenMau,
                TenSize = x.SanPhamBienThe?.Size?.TenSize,
                GiaBan = x.SanPhamBienThe?.SanPham?.GiaBan ?? 0,
                SoLuong = x.SoLuong ?? 0
            });
        }

        public async Task<bool> AddToCartAsync(int userId, ThemGioHangDto dto)
        {
            // Kiểm tra tính hợp lệ của SanPhamBienTheId
            if (dto.SanPhamBienTheId <= 0) return false;

            // Kiểm tra xem biến thể này có tồn tại không
            var bienThe = await _bienTheRepo.GetByIdAsync(dto.SanPhamBienTheId);
            if (bienThe == null) return false;

            var allCart = await _gioHangRepo.GetAllAsync();
            var existingItem = allCart.FirstOrDefault(x => x.UserId == userId && x.SanPhamBienTheId == dto.SanPhamBienTheId);

            if (existingItem != null)
            {
                // Nếu đã có trong giỏ -> Cộng dồn số lượng
                existingItem.SoLuong += dto.SoLuong;
                await _gioHangRepo.UpdateAsync(existingItem);
            }
            else
            {
                // Nếu chưa có -> Thêm mới
                var newItem = new GioHang
                {
                    UserId = userId,
                    SanPhamBienTheId = dto.SanPhamBienTheId,
                    SoLuong = dto.SoLuong,
                    NgayThem = DateTime.Now
                };
                await _gioHangRepo.AddAsync(newItem);
            }
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int cartId)
        {
            var cartItem = await _gioHangRepo.GetByIdAsync(cartId);
            // Ktra bảo mật: Chỉ cho phép xóa giỏ hàng của chính mình
            if (cartItem == null || cartItem.UserId != userId) return false;

            await _gioHangRepo.DeleteAsync(cartId);
            return true;
        }
    }
}
