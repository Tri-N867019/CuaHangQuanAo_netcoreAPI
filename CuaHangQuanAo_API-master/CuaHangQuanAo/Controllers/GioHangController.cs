using CuaHangQuanAo.Application.DTOs.GioHang;
using CuaHangQuanAo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class GioHangController : ControllerBase
    {
        private readonly IGioHangService _gioHangService;

        public GioHangController(IGioHangService gioHangService)
        {
            _gioHangService = gioHangService;
        }

        // Hàm bí mật: Lấy UserId của người đang đăng nhập từ vé JWT
        private int GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdStr ?? "0");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = GetCurrentUserId();
            var data = await _gioHangService.GetCartByUserIdAsync(userId);
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] ThemGioHangDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _gioHangService.AddToCartAsync(userId, dto);
            if (!result) return BadRequest("Vui lòng chọn đầy đủ Màu sắc và Kích cỡ trước khi thêm vào giỏ hàng!");
            return Ok("Đã thêm vào giỏ hàng thành công!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _gioHangService.RemoveFromCartAsync(userId, id);
            if (!result) return NotFound("Không tìm thấy món hàng này trong giỏ của bạn.");
            return Ok("Đã xóa khỏi giỏ hàng!");
        }
    }
}
