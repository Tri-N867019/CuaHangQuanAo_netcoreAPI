using CuaHangQuanAo.Application.DTOs.HoaDon;
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
    public class HoaDonService : IHoaDonService
    {
        private readonly IGenericRepository<HoaDon> _hoaDonRepo;
        private readonly IGenericRepository<ChiTietHoaDon> _chiTietRepo;
        private readonly IGenericRepository<GioHang> _gioHangRepo;
        private readonly IGenericRepository<SanPhamBienThe> _bienTheRepo;

        public HoaDonService(
            IGenericRepository<HoaDon> hoaDonRepo,
            IGenericRepository<ChiTietHoaDon> chiTietRepo,
            IGenericRepository<GioHang> gioHangRepo,
            IGenericRepository<SanPhamBienThe> bienTheRepo)
        {
            _hoaDonRepo = hoaDonRepo;
            _chiTietRepo = chiTietRepo;
            _gioHangRepo = gioHangRepo;
            _bienTheRepo = bienTheRepo;
        }

        public async Task<DatHangResult> DatHangAsync(int userId, DatHangDto dto)
        {
            var allCart = await _gioHangRepo.GetAllAsync(
                x => x.SanPhamBienThe!,
                x => x.SanPhamBienThe!.SanPham!,
                x => x.SanPhamBienThe!.MauSac!,
                x => x.SanPhamBienThe!.Size!
            );
            var myCart = allCart.Where(x => x.UserId == userId).ToList();

            // Nếu có truyền danh sách ID được chọn, thì lọc lại giỏ hàng
            if (dto.CartItemIds != null && dto.CartItemIds.Any())
            {
                myCart = myCart.Where(x => dto.CartItemIds.Contains(x.Id)).ToList();
            }


            if (!myCart.Any()) 
                return new DatHangResult { Success = false, Message = "Giỏ hàng trống!" };

            // 1.5 Kiểm tra tồn kho và Trừ kho thực tế
            foreach (var item in myCart)
            {
                if (item.SanPhamBienTheId == null) continue;

                // Lấy dữ liệu biến thể mới nhất từ database bằng repository để đảm bảo tính chính xác
                var variant = await _bienTheRepo.GetByIdAsync(item.SanPhamBienTheId.Value);
                
                if (variant == null)
                    return new DatHangResult { Success = false, Message = $"Sản phẩm biến thể #{item.SanPhamBienTheId} không tồn tại!" };

                if ((variant.SoLuongTon ?? 0) < (item.SoLuong ?? 0))
                {
                    return new DatHangResult 
                    { 
                        Success = false, 
                        Message = $"Sản phẩm '{item.SanPhamBienThe?.SanPham?.Ten} - {item.SanPhamBienThe?.MauSac?.TenMau}/{item.SanPhamBienThe?.Size?.TenSize}' hiện chỉ còn {variant.SoLuongTon} sản phẩm. Vui lòng cập nhật lại giỏ hàng!" 
                    };
                }

                // Thực hiện trừ kho ngay lập tức để giữ chỗ
                variant.SoLuongTon -= (item.SoLuong ?? 0);
                await _bienTheRepo.UpdateAsync(variant);
            }

            // 2. Tính tổng tiền hàng
            decimal tongTienHang = 0;
            foreach (var item in myCart)
            {
                var gia = item.SanPhamBienThe?.SanPham?.GiaBan ?? 0;
                tongTienHang += gia * (item.SoLuong ?? 0);
            }


            // 3. Tạo Hóa Đơn mới
            var hoaDon = new HoaDon
            {
                UserId = userId,
                NgayTao = DateTime.Now,
                TongTien = tongTienHang,
                PhiVanChuyen = 30000, 
                TenNguoiNhan = dto.TenNguoiNhan,
                SDTNhanHang = dto.SDTNhanHang,
                DiaChiGiaoHang = dto.DiaChiGiaoHang,
                PhuongThucThanhToan = dto.PhuongThucThanhToan,
                TrangThai = 0, 
                TrangThaiThanhToan = 0, 
                GhiChu = dto.GhiChu
            };
            await _hoaDonRepo.AddAsync(hoaDon);

            // 4. Tạo các Chi Tiết Hóa Đơn (Snapshot dữ liệu)
            foreach (var item in myCart)
            {
                var giaBanLive = item.SanPhamBienThe?.SanPham?.GiaBan ?? 0;
                var chiTiet = new ChiTietHoaDon
                {
                    HoaDonId = hoaDon.Id,
                    SanPhamBienTheId = item.SanPhamBienTheId,
                    TenSanPham = item.SanPhamBienThe?.SanPham?.Ten,
                    TenMau = item.SanPhamBienThe?.MauSac?.TenMau,
                    TenSize = item.SanPhamBienThe?.Size?.TenSize,
                    Soluong = item.SoLuong,
                    GiaBan = giaBanLive,
                    ThanhTien = giaBanLive * item.SoLuong
                };
                await _chiTietRepo.AddAsync(chiTiet);

                // 5. Xóa món đồ này khỏi Giỏ hàng
                await _gioHangRepo.DeleteAsync(item.Id);
            }

            return new DatHangResult { Success = true, Message = "Đặt hàng thành công!", OrderId = hoaDon.Id };
        }

        public async Task<IEnumerable<HoaDonDto>> GetLichSuMuaHangAsync(int userId)
        {
            // Lấy hóa đơn kèm theo chi tiết
            var allOrders = await _hoaDonRepo.GetAllAsync(x => x.ChiTietHoaDons!);
            var myOrders = allOrders.Where(x => x.UserId == userId).OrderByDescending(x => x.NgayTao);

            return myOrders.Select(x => new HoaDonDto
            {
                Id = x.Id,
                NgayTao = x.NgayTao,
                TongTien = x.TongTien,
                PhiVanChuyen = x.PhiVanChuyen,
                TenNguoiNhan = x.TenNguoiNhan,
                SDTNhanHang = x.SDTNhanHang,
                DiaChiGiaoHang = x.DiaChiGiaoHang,
                TrangThai = x.TrangThai,
                PhuongThucThanhToan = x.PhuongThucThanhToan,
                ChiTiet = x.ChiTietHoaDons!.Select(ct => new ChiTietHoaDonDto
                {
                    TenSanPham = ct.TenSanPham,
                    TenMau = ct.TenMau,
                    TenSize = ct.TenSize,
                    Soluong = ct.Soluong,
                    GiaBan = ct.GiaBan,
                    ThanhTien = ct.ThanhTien
                }).ToList()
            });
        }

        public async Task<IEnumerable<HoaDonDto>> GetAllOrdersAsync()
        {
            // Lấy tất cả hóa đơn cho admin (sắp xếp mới nhất trước)
            var allOrders = await _hoaDonRepo.GetAllAsync(x => x.ChiTietHoaDons!);
            var orders = allOrders.OrderByDescending(x => x.NgayTao);

            return orders.Select(x => new HoaDonDto
            {
                Id = x.Id,
                NgayTao = x.NgayTao,
                TongTien = x.TongTien,
                PhiVanChuyen = x.PhiVanChuyen,
                TenNguoiNhan = x.TenNguoiNhan,
                SDTNhanHang = x.SDTNhanHang,
                DiaChiGiaoHang = x.DiaChiGiaoHang,
                TrangThai = x.TrangThai,
                PhuongThucThanhToan = x.PhuongThucThanhToan,
                ChiTiet = x.ChiTietHoaDons!.Select(ct => new ChiTietHoaDonDto
                {
                    TenSanPham = ct.TenSanPham,
                    TenMau = ct.TenMau,
                    TenSize = ct.TenSize,
                    Soluong = ct.Soluong,
                    GiaBan = ct.GiaBan,
                    ThanhTien = ct.ThanhTien
                }).ToList()
            });
        }

        public async Task<HoaDonDto> GetOrderByIdAsync(int orderId)
        {
            var allOrders = await _hoaDonRepo.GetAllAsync(x => x.ChiTietHoaDons!);
            var order = allOrders.FirstOrDefault(x => x.Id == orderId);

            if (order == null) return null;

            return new HoaDonDto
            {
                Id = order.Id,
                NgayTao = order.NgayTao,
                TongTien = order.TongTien,
                PhiVanChuyen = order.PhiVanChuyen,
                TenNguoiNhan = order.TenNguoiNhan,
                SDTNhanHang = order.SDTNhanHang,
                DiaChiGiaoHang = order.DiaChiGiaoHang,
                TrangThai = order.TrangThai,
                PhuongThucThanhToan = order.PhuongThucThanhToan,
                ChiTiet = order.ChiTietHoaDons!.Select(ct => new ChiTietHoaDonDto
                {
                    TenSanPham = ct.TenSanPham,
                    TenMau = ct.TenMau,
                    TenSize = ct.TenSize,
                    Soluong = ct.Soluong,
                    GiaBan = ct.GiaBan,
                    ThanhTien = ct.ThanhTien
                }).ToList()
            };
        }

        public async Task<bool> ApproveOrderAsync(int orderId)
        {
            var order = await _hoaDonRepo.GetByIdAsync(orderId);
            if (order == null || order.TrangThai != 0) return false; // Chỉ duyệt đơn đang chờ (0)

            // Lưu ý: Logic trừ kho đã được thực hiện ngay khi khách Đặt hàng (DatHangAsync)
            // Để đảm bảo "giữ chỗ" sản phẩm cho khách ngay lập tức.
            // Do đó ở bước Duyệt này không cần trừ kho nữa để tránh trừ 2 lần.

            order.TrangThai = 1; // 1: Đã duyệt
            await _hoaDonRepo.UpdateAsync(order);
            return true;
        }

        public async Task<bool> RejectOrderAsync(int orderId)
        {
            var order = await _hoaDonRepo.GetByIdAsync(orderId);
            if (order == null || order.TrangThai == 4) return false; // Đã hủy rồi thì thôi

            // Vì kho đã trừ lúc Đặt hàng, nên khi Hủy/Từ chối bất kể lúc nào cũng phải hoàn kho
            var details = (await _chiTietRepo.GetAllAsync()).Where(x => x.HoaDonId == orderId).ToList();
            foreach (var item in details)
            {
                if (item.SanPhamBienTheId.HasValue)
                {
                    var bienThe = await _bienTheRepo.GetByIdAsync(item.SanPhamBienTheId.Value);
                    if (bienThe != null)
                    {
                        bienThe.SoLuongTon += (item.Soluong ?? 0);
                        await _bienTheRepo.UpdateAsync(bienThe);
                    }
                }
            }

            order.TrangThai = 4; // 4: Đã hủy/Từ chối
            await _hoaDonRepo.UpdateAsync(order);
            return true;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, int newStatus)
        {
            var allOrders = await _hoaDonRepo.GetAllAsync();
            var order = await _hoaDonRepo.GetByIdAsync(orderId);
            if (order == null) return false;

            order.TrangThai = (byte)newStatus;
            await _hoaDonRepo.UpdateAsync(order);
            return true;
        }

        public async Task<bool> MarkAsShippedAsync(int orderId)
        {
            var allOrders = await _hoaDonRepo.GetAllAsync();
            var order = allOrders.FirstOrDefault(x => x.Id == orderId);

            if (order == null) return false;

            order.TrangThai = 2; // 2: Đang vận chuyển
            await _hoaDonRepo.UpdateAsync(order);
            return true;
        }

        public async Task<bool> MarkAsDeliveredAsync(int orderId)
        {
            var allOrders = await _hoaDonRepo.GetAllAsync();
            var order = allOrders.FirstOrDefault(x => x.Id == orderId);

            if (order == null) return false;

            order.TrangThai = 3; // 3: Đã giao
            await _hoaDonRepo.UpdateAsync(order);
            return true;
        }

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
            var order = await _hoaDonRepo.GetByIdAsync(orderId);
            
            // Kiểm tra: Tồn tại + Đúng chủ + Đang chờ duyệt (0)
            if (order == null || order.UserId != userId || order.TrangThai != 0) 
                return false;

            // Hoàn kho khi người dùng tự hủy đơn
            var details = (await _chiTietRepo.GetAllAsync()).Where(x => x.HoaDonId == orderId).ToList();
            foreach (var item in details)
            {
                if (item.SanPhamBienTheId.HasValue)
                {
                    var bienThe = await _bienTheRepo.GetByIdAsync(item.SanPhamBienTheId.Value);
                    if (bienThe != null)
                    {
                        bienThe.SoLuongTon += (item.Soluong ?? 0);
                        await _bienTheRepo.UpdateAsync(bienThe);
                    }
                }
            }

            order.TrangThai = 4; // 4: Đã hủy
            await _hoaDonRepo.UpdateAsync(order);
            return true;
        }
    }
}
