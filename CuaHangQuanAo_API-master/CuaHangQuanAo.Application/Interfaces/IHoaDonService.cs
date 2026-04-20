using CuaHangQuanAo.Application.DTOs.HoaDon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Interfaces
{
    public interface IHoaDonService
    {
        Task<DatHangResult> DatHangAsync(int userId, DatHangDto dto); // Chốt đơn
        Task<IEnumerable<HoaDonDto>> GetLichSuMuaHangAsync(int userId); // Xem lịch sử mua hàng
        Task<IEnumerable<HoaDonDto>> GetAllOrdersAsync(); // Lấy tất cả đơn hàng
        Task<HoaDonDto> GetOrderByIdAsync(int orderId); // Xem chi tiết đơn hàng
        Task<bool> ApproveOrderAsync(int orderId); // Duyệt đơn hàng
        Task<bool> RejectOrderAsync(int orderId); // Hủy đơn hàng
        Task<bool> UpdateOrderStatusAsync(int orderId, int newStatus); // Đổi trạng thái đơn hàng
        Task<bool> MarkAsShippedAsync(int orderId); // Xác nhận đang giao
        Task<bool> MarkAsDeliveredAsync(int orderId); // Xác nhận đã giao
        Task<bool> CancelOrderAsync(int orderId, int userId); // Người dùng tự hủy đơn
    }
}
