using CuaHangQuanAo.Application.DTOs.HoaDon;
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
    public class HoaDonController : ControllerBase
    {
        private readonly IHoaDonService _hoaDonService;

        public HoaDonController(IHoaDonService hoaDonService)
        {
            _hoaDonService = hoaDonService;
        }

        private int GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdStr ?? "0");
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] DatHangDto dto)
        {
            if (User.IsInRole("Admin1") || User.IsInRole("Admin"))
            {
                return BadRequest("Tài khoản quản trị không thể mua hàng. Vui lòng sử dụng tài khoản Khách hàng.");
            }

            var userId = GetCurrentUserId();
            var result = await _hoaDonService.DatHangAsync(userId, dto);

            if (!result.Success) return BadRequest(result.Message);

            return Ok(new { message = "Đặt hàng thành công!", orderId = result.OrderId });
        }

        [HttpGet("lich-su")]
        public async Task<IActionResult> GetLichSuMuaHang()
        {
            var userId = GetCurrentUserId();
            var data = await _hoaDonService.GetLichSuMuaHangAsync(userId);
            return Ok(data);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin1,Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var data = await _hoaDonService.GetAllOrdersAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var data = await _hoaDonService.GetOrderByIdAsync(id);
            if (data == null) return NotFound("Không tìm thấy đơn hàng!");
            return Ok(data);
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin1,Admin,NhanVien")]
        public async Task<IActionResult> ApproveOrder(int id)
        {
            var result = await _hoaDonService.ApproveOrderAsync(id);
            if (!result) return NotFound("Không tìm thấy đơn hàng!");
            return Ok("Duyệt đơn hàng thành công!");
        }

        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin1,Admin,NhanVien")]
        public async Task<IActionResult> RejectOrder(int id)
        {
            var result = await _hoaDonService.RejectOrderAsync(id);
            if (!result) return NotFound("Không tìm thấy đơn hàng!");
            return Ok("Từ chối đơn hàng thành công!");
        }

        [HttpPut("{id}/shipped")]
        [Authorize(Roles = "Admin1,Admin")]
        public async Task<IActionResult> MarkAsShipped(int id)
        {
            var result = await _hoaDonService.MarkAsShippedAsync(id);
            if (!result) return NotFound("Không tìm thấy đơn hàng!");
            return Ok("Cập nhật trạng thái: Đang vận chuyển!");
        }

        [HttpPut("{id}/delivered")]
        [Authorize(Roles = "Admin1,Admin")]
        public async Task<IActionResult> MarkAsDelivered(int id)
        {
            var result = await _hoaDonService.MarkAsDeliveredAsync(id);
            if (!result) return NotFound("Không tìm thấy đơn hàng!");
            return Ok("Cập nhật trạng thái: Đã giao!");
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin1,Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] int newStatus)
        {
            var result = await _hoaDonService.UpdateOrderStatusAsync(id, newStatus);
            if (!result) return NotFound("Không tìm thấy đơn hàng!");
            return Ok("Cập nhật trạng thái đơn hàng thành công!");
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _hoaDonService.CancelOrderAsync(id, userId);
            if (!result) return BadRequest("Không thể hủy đơn hàng này (Đã được duyệt hoặc không phải đơn của bạn)!");
            return Ok("Hủy đơn hàng thành công!");
        }
    }
}
