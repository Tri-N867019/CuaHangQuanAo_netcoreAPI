using CuaHangQuanAo.Application.DTOs; // Thêm thư viện chứa ProductStatsDto nếu cần
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
    public class ThongKeService : IThongKeService
    {
        private readonly IGenericRepository<HoaDon> _hoaDonRepo;
        private readonly IGenericRepository<Users> _userRepo;
        private readonly IGenericRepository<SanPham> _sanPhamRepo;
        private readonly IGenericRepository<LoaiSP> _loaiSPRepo;
        private readonly IGenericRepository<ThuongHieu> _thuongHieuRepo;
        private readonly IGenericRepository<ChiTietHoaDon> _chiTietRepo;

        public ThongKeService(
            IGenericRepository<HoaDon> hoaDonRepo,
            IGenericRepository<Users> userRepo,
            IGenericRepository<SanPham> sanPhamRepo,
            IGenericRepository<LoaiSP> loaiSPRepo,
            IGenericRepository<ThuongHieu> thuongHieuRepo,
            IGenericRepository<ChiTietHoaDon> chiTietRepo)
        {
            _hoaDonRepo = hoaDonRepo;
            _userRepo = userRepo;
            _sanPhamRepo = sanPhamRepo;
            _loaiSPRepo = loaiSPRepo;
            _thuongHieuRepo = thuongHieuRepo;
            _chiTietRepo = chiTietRepo;
        }

        public async Task<int> GetTotalOrdersAsync()
        {
            var orders = await _hoaDonRepo.GetAllAsync();
            return orders.Count();
        }

        public async Task<int> GetPendingOrdersAsync()
        {
            var orders = await _hoaDonRepo.GetAllAsync();
            return orders.Where(x => x.TrangThai == 0).Count(); // 0: Chờ duyệt
        }

        public async Task<int> GetTotalUsersAsync()
        {
            var users = await _userRepo.GetAllAsync();
            return users.Count();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            var orders = await _hoaDonRepo.GetAllAsync();
            return orders.Where(x => x.TrangThai >= 1).Sum(x => x.TongTien ?? 0); // Chỉ tính đơn đã duyệt
        }

        public async Task<decimal> GetRevenueToday()
        {
            var orders = await _hoaDonRepo.GetAllAsync();
            var today = DateTime.Now.Date;
            // SỬA: Kiểm tra HasValue và dùng .Value.Date
            return orders.Where(x => x.NgayTao.HasValue && x.NgayTao.Value.Date == today && x.TrangThai >= 1)
                         .Sum(x => x.TongTien ?? 0);
        }

        public async Task<decimal> GetRevenueThisMonth()
        {
            var orders = await _hoaDonRepo.GetAllAsync();
            var today = DateTime.Now;
            // SỬA: Kiểm tra HasValue và dùng .Value.Year / .Value.Month
            return orders.Where(x => x.NgayTao.HasValue && x.NgayTao.Value.Year == today.Year && x.NgayTao.Value.Month == today.Month && x.TrangThai >= 1)
                         .Sum(x => x.TongTien ?? 0);
        }

        public async Task<decimal> GetRevenueThisYear()
        {
            var orders = await _hoaDonRepo.GetAllAsync();
            var today = DateTime.Now;
            // SỬA: Kiểm tra HasValue và dùng .Value.Year
            return orders.Where(x => x.NgayTao.HasValue && x.NgayTao.Value.Year == today.Year && x.TrangThai >= 1)
                         .Sum(x => x.TongTien ?? 0);
        }

        public async Task<Dictionary<string, int>> GetOrderStatusDistributionAsync()
        {
            var orders = await _hoaDonRepo.GetAllAsync();
            var distribution = new Dictionary<string, int>
            {
                { "Chờ duyệt", orders.Where(x => x.TrangThai == 0).Count() },
                { "Đã duyệt", orders.Where(x => x.TrangThai == 1).Count() },
                { "Đang vận chuyển", orders.Where(x => x.TrangThai == 2).Count() },
                { "Đã giao", orders.Where(x => x.TrangThai == 3).Count() },
                { "Đã hủy", orders.Where(x => x.TrangThai == 4).Count() } // Đổi tên nhãn cho đồng bộ với frontend
            };
            return distribution;
        }

        public async Task<Dictionary<string, decimal>> GetMonthlySalesAsync()
        {
            var orders = await _hoaDonRepo.GetAllAsync();
            var salesByMonth = new Dictionary<string, decimal>();
            var currentYear = DateTime.Now.Year;

            for (int month = 1; month <= 12; month++)
            {
                // SỬA: Kiểm tra HasValue và lấy doanh thu theo từng tháng (của năm hiện tại)
                var monthSales = orders.Where(x => x.NgayTao.HasValue && x.NgayTao.Value.Year == currentYear && x.NgayTao.Value.Month == month && x.TrangThai >= 1)
                                       .Sum(x => x.TongTien ?? 0);
                var monthName = GetMonthName(month);
                salesByMonth[monthName] = monthSales;
            }

            return salesByMonth;
        }

        public async Task<List<ProductStatsDto>> GetTopSellingProductsAsync(int topN = 10)
        {
            var chiTiets = await _chiTietRepo.GetAllAsync();
            var orders = await _hoaDonRepo.GetAllAsync();

            // Lọc chỉ các id đơn đã duyệt
            var approvedOrderIds = orders.Where(x => x.TrangThai >= 1).Select(x => x.Id).ToList();

            var topProducts = chiTiets
                // SỬA LỖI CS1503: Kiểm tra HasValue trước khi gọi .Value
                .Where(x => x.HoaDonId.HasValue && approvedOrderIds.Contains(x.HoaDonId.Value))
                .GroupBy(x => x.TenSanPham)
                .Select(g => new ProductStatsDto
                {
                    // SỬA CẢNH BÁO CS8618: Gán giá trị mặc định nếu TenSanPham bị null
                    TenSanPham = g.Key ?? "Sản phẩm không tên",
                    SoLuongBan = g.Sum(x => x.Soluong ?? 0),
                    DoanhSo = g.Sum(x => x.ThanhTien ?? 0)
                })
                .OrderByDescending(x => x.SoLuongBan)
                .Take(topN)
                .ToList();

            return topProducts;
        }

        public async Task<int> GetTotalProductsAsync()
        {
            var products = await _sanPhamRepo.GetAllAsync();
            return products.Count();
        }

        public async Task<int> GetTotalCategoriesAsync()
        {
            var categories = await _loaiSPRepo.GetAllAsync();
            return categories.Count();
        }

        public async Task<int> GetTotalBrandsAsync()
        {
            var brands = await _thuongHieuRepo.GetAllAsync();
            return brands.Count();
        }

        private string GetMonthName(int month)
        {
            return month switch
            {
                1 => "Tháng 1",
                2 => "Tháng 2",
                3 => "Tháng 3",
                4 => "Tháng 4",
                5 => "Tháng 5",
                6 => "Tháng 6",
                7 => "Tháng 7",
                8 => "Tháng 8",
                9 => "Tháng 9",
                10 => "Tháng 10",
                11 => "Tháng 11",
                12 => "Tháng 12",
                _ => ""
            };
        }
    }
}