using CuaHangQuanAo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin1,Admin,NhanVien")]
    public class ThongKeController : ControllerBase
    {
        private readonly IThongKeService _thongKeService;

        public ThongKeController(IThongKeService thongKeService)
        {
            _thongKeService = thongKeService;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var overview = new
            {
                TotalOrders = await _thongKeService.GetTotalOrdersAsync(),
                PendingOrders = await _thongKeService.GetPendingOrdersAsync(),
                TotalUsers = await _thongKeService.GetTotalUsersAsync(),
                TotalProducts = await _thongKeService.GetTotalProductsAsync(),
                TotalCategories = await _thongKeService.GetTotalCategoriesAsync(),
                TotalBrands = await _thongKeService.GetTotalBrandsAsync(),
                TotalRevenue = await _thongKeService.GetTotalRevenueAsync(),
                RevenueToday = await _thongKeService.GetRevenueToday(),
                RevenueThisMonth = await _thongKeService.GetRevenueThisMonth(),
                RevenueThisYear = await _thongKeService.GetRevenueThisYear()
            };
            return Ok(overview);
        }

        [HttpGet("order-status-distribution")]
        public async Task<IActionResult> GetOrderStatusDistribution()
        {
            var distribution = await _thongKeService.GetOrderStatusDistributionAsync();
            return Ok(distribution);
        }

        [HttpGet("monthly-sales")]
        public async Task<IActionResult> GetMonthlySales()
        {
            var sales = await _thongKeService.GetMonthlySalesAsync();
            return Ok(sales);
        }

        [HttpGet("top-selling-products")]
        public async Task<IActionResult> GetTopSellingProducts([FromQuery] int topN = 10)
        {
            var products = await _thongKeService.GetTopSellingProductsAsync(topN);
            return Ok(products);
        }

        [HttpGet("total-orders")]
        public async Task<IActionResult> GetTotalOrders()
        {
            var count = await _thongKeService.GetTotalOrdersAsync();
            return Ok(new { count });
        }

        [HttpGet("pending-orders")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var count = await _thongKeService.GetPendingOrdersAsync();
            return Ok(new { count });
        }

        [HttpGet("total-users")]
        public async Task<IActionResult> GetTotalUsers()
        {
            var count = await _thongKeService.GetTotalUsersAsync();
            return Ok(new { count });
        }

        [HttpGet("total-revenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var revenue = await _thongKeService.GetTotalRevenueAsync();
            return Ok(new { revenue });
        }

        [HttpGet("revenue-today")]
        public async Task<IActionResult> GetRevenueToday()
        {
            var revenue = await _thongKeService.GetRevenueToday();
            return Ok(new { revenue });
        }

        [HttpGet("revenue-this-month")]
        public async Task<IActionResult> GetRevenueThisMonth()
        {
            var revenue = await _thongKeService.GetRevenueThisMonth();
            return Ok(new { revenue });
        }

        [HttpGet("revenue-this-year")]
        public async Task<IActionResult> GetRevenueThisYear()
        {
            var revenue = await _thongKeService.GetRevenueThisYear();
            return Ok(new { revenue });
        }
    }
}
