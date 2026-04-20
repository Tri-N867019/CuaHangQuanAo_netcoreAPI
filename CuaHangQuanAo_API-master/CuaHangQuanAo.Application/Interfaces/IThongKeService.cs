using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Interfaces
{
    public interface IThongKeService
    {
        // Thống kê tổng quan
        Task<int> GetTotalOrdersAsync(); // Tổng số đơn hàng
        Task<int> GetPendingOrdersAsync(); // Số đơn chờ duyệt
        Task<int> GetTotalUsersAsync(); // Tổng số tài khoản
        Task<decimal> GetTotalRevenueAsync(); // Tổng doanh thu
        Task<decimal> GetRevenueToday(); // Doanh thu hôm nay
        Task<decimal> GetRevenueThisMonth(); // Doanh thu tháng này
        Task<decimal> GetRevenueThisYear(); // Doanh thu năm nay

        // Chi tiết thống kê
        Task<Dictionary<string, int>> GetOrderStatusDistributionAsync(); // Số lượng đơn theo trạng thái
        Task<Dictionary<string, decimal>> GetMonthlySalesAsync(); // Doanh thu theo tháng
        Task<List<ProductStatsDto>> GetTopSellingProductsAsync(int topN = 10); // Top sản phẩm bán chạy
        Task<int> GetTotalProductsAsync(); // Tổng số sản phẩm
        Task<int> GetTotalCategoriesAsync(); // Tổng số danh mục
        Task<int> GetTotalBrandsAsync(); // Tổng số thương hiệu
    }

    public class ProductStatsDto
    {
        public int Id { get; set; }
        public string? TenSanPham { get; set; }
        public int SoLuongBan { get; set; }
        public decimal DoanhSo { get; set; }
    }

}