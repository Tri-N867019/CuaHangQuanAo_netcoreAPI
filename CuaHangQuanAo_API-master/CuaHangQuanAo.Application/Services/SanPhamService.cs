using CuaHangQuanAo.Application.DTOs.SanPham;
using CuaHangQuanAo.Application.Interfaces;
using CuaHangQuanAo.Domain.Entities;
using CuaHangQuanAo.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Services
{
    public class SanPhamService : ISanPhamService
    {
        private readonly IGenericRepository<SanPham> _repository;
        private readonly IAnhSanPhamService _anhSanPhamService;
        private readonly ISanPhamBienTheService _sanPhamBienTheService;
        private readonly IMauSacService _mauSacService;
        private readonly ISizeService _sizeService;

        public SanPhamService(
            IGenericRepository<SanPham> repository,
            IAnhSanPhamService anhSanPhamService,
            ISanPhamBienTheService sanPhamBienTheService,
            IMauSacService mauSacService,
            ISizeService sizeService)
        {
            _repository = repository;
            _anhSanPhamService = anhSanPhamService;
            _sanPhamBienTheService = sanPhamBienTheService;
            _mauSacService = mauSacService;
            _sizeService = sizeService;
        }

        public async Task<IEnumerable<SanPhamDto>> GetAllAsync(string? searchKeyword = null)
        {
            // Sử dụng string includes để lấy được MauSac sâu bên trong
            var includes = new string[] { "LoaiSP", "ThuongHieu", "AnhSanPhams", "SanPhamBienThes.MauSac" };
            var dataIEnumerable = await _repository.GetAllAsync(includes);
            
            // Lọc theo từ khóa nếu có
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                dataIEnumerable = dataIEnumerable.Where(x => 
                    x.Ten!.Contains(searchKeyword, System.StringComparison.OrdinalIgnoreCase));
            }

            //  trả về 
            return dataIEnumerable.Select(x => new SanPhamDto
            {
                Id = x.Id,
                Ten = x.Ten,
                MoTa = x.MoTa,
                GiaBan = x.GiaBan,
                LoaiId = x.LoaiId ?? 0,
                ThuongHieuId = x.ThuongHieuId ?? 0,
                GioiTinh = x.GioiTinh,

                GiaNhap = x.GiaNhap,
                TrangThaiSanPham = x.TrangThaiSanPham,
                TrangThaiHoatDong = x.TrangThaiHoatDong,
                SoLuong = x.SanPhamBienThes?.Sum(b => b.SoLuongTon ?? 0) ?? 0,

                TenLoai = x.LoaiSP?.TenLSP,
                TenThuongHieu = x.ThuongHieu?.TenTH,
                
                // Lấy danh sách ID màu để lọc
                MauIds = x.SanPhamBienThes != null 
                    ? x.SanPhamBienThes.Where(b => b.MauSac != null).Select(b => b.MauId ?? 0).Distinct().ToList() 
                    : new List<int>(),

                // Lấy danh sách mã màu DUY NHẤT để hiển thị ở Frontend
                MaMaus = x.SanPhamBienThes != null 
                    ? x.SanPhamBienThes.Where(b => b.MauSac != null).Select(b => b.MauSac!.MaMau!).Distinct().ToList() 
                    : new List<string>(),

                // Lấy ảnh đầu tiên của sản phẩm làm ảnh đại diện
                HinhAnh = x.AnhSanPhams != null && x.AnhSanPhams.Any() && !string.IsNullOrEmpty(x.AnhSanPhams.First().TenAnhSP)
                    ? (x.AnhSanPhams.First().TenAnhSP!.StartsWith("http") 
                        ? x.AnhSanPhams.First().TenAnhSP 
                        : (x.AnhSanPhams.First().TenAnhSP!.StartsWith("/") 
                            ? x.AnhSanPhams.First().TenAnhSP 
                            : "/uploads/" + x.AnhSanPhams.First().TenAnhSP)) 
                    : "/img/no-image.png",

                KhuyenMai = x.KhuyenMai

            });
        }

        public async Task<int> AddAsync(CreateUpdateSanPhamDto dto)
        {
            var sp = new SanPham
            {
                Ten = dto.Ten,
                MoTa = dto.MoTa,
                GiaBan = dto.GiaBan,
                GiaNhap = dto.GiaNhap,
                KhuyenMai = dto.KhuyenMai,
                HuongDan = dto.HuongDan,
                ThanhPhan = dto.ThanhPhan,
                TrangThaiSanPham = dto.TrangThaiSanPham,
                TrangThaiHoatDong = dto.TrangThaiHoatDong ?? true, // Mặc định là Hoạt động
                GioiTinh = dto.GioiTinh,
                LoaiId = dto.LoaiId,
                ThuongHieuId = dto.ThuongHieuId,
                NgayTao = System.DateTime.Now
            };
            await _repository.AddAsync(sp);
            return sp.Id;
        }

        public async Task<SanPhamDto?> GetByIdAsync(int id)
        {
            var data = await _repository.GetAllAsync(x => x.LoaiSP!, x => x.ThuongHieu!);
            var sp = data.FirstOrDefault(x => x.Id == id);
            if (sp == null) return null;

            return new SanPhamDto
            {
                Id = sp.Id,
                Ten = sp.Ten,
                MoTa = sp.MoTa,
                GiaBan = sp.GiaBan,
                KhuyenMai = sp.KhuyenMai,
                GiaNhap = sp.GiaNhap,
                TrangThaiSanPham = sp.TrangThaiSanPham,
                TrangThaiHoatDong = sp.TrangThaiHoatDong,
                LoaiId = sp.LoaiId ?? 0,
                ThuongHieuId = sp.ThuongHieuId ?? 0,
                TenLoai = sp.LoaiSP?.TenLSP,
                TenThuongHieu = sp.ThuongHieu?.TenTH
            };
        }

        public async Task UpdateAsync(int id, CreateUpdateSanPhamDto dto)
        {
            var sp = await _repository.GetByIdAsync(id);
            if (sp != null)
            {
                sp.Ten = dto.Ten;
                sp.MoTa = dto.MoTa;
                sp.GiaBan = dto.GiaBan;
                sp.GiaNhap = dto.GiaNhap;
                sp.KhuyenMai = dto.KhuyenMai;
                sp.HuongDan = dto.HuongDan;
                sp.ThanhPhan = dto.ThanhPhan;
                sp.TrangThaiSanPham = dto.TrangThaiSanPham;
                sp.TrangThaiHoatDong = dto.TrangThaiHoatDong ?? sp.TrangThaiHoatDong;
                sp.GioiTinh = dto.GioiTinh;
                sp.LoaiId = dto.LoaiId;
                sp.ThuongHieuId = dto.ThuongHieuId;
                sp.NgayCapNhat = System.DateTime.Now;

                await _repository.UpdateAsync(sp);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<SanPhamDetailDto?> GetChiTietSanPhamAsync(int id, string baseUrl)
        {
            // 1. Lấy thông tin sản phẩm cơ bản
            var includes = new string[] { "LoaiSP", "ThuongHieu", "AnhSanPhams" };
            var dataSP = await _repository.GetAllAsync(includes);
            var sp = dataSP.FirstOrDefault(x => x.Id == id);
            if (sp == null) return null;

            // 2. Lấy danh sách biến thể
            var listBienThe = await _sanPhamBienTheService.GetBySanPhamIdAsync(id);
            
            // 3. Lấy tất cả Màu và Size để JOIN thủ công (Đảm bảo luôn có tên dù Include thất bại)
            var allColors = await _mauSacService.GetAllAsync();
            var allSizes = await _sizeService.GetAllAsync();

            return new SanPhamDetailDto
            {
                Id = sp.Id,
                Ten = sp.Ten,
                MoTa = sp.MoTa,
                GiaBan = sp.GiaBan,
                KhuyenMai = sp.KhuyenMai,
                HuongDan = sp.HuongDan,
                ThanhPhan = sp.ThanhPhan,
                TenLoai = sp.LoaiSP?.TenLSP,
                TenThuongHieu = sp.ThuongHieu?.TenTH,
                DanhSachAnh = sp.AnhSanPhams != null && sp.AnhSanPhams.Any() 
                    ? sp.AnhSanPhams.Select(a => 
                        !string.IsNullOrEmpty(a.TenAnhSP) && a.TenAnhSP.StartsWith("http") 
                        ? a.TenAnhSP 
                        : (a.TenAnhSP != null && (a.TenAnhSP.StartsWith("/") || a.TenAnhSP.StartsWith("uploads/")) 
                            ? $"{baseUrl}/{(a.TenAnhSP.StartsWith("/") ? a.TenAnhSP.Substring(1) : a.TenAnhSP)}" 
                            : $"{baseUrl}/uploads/{a.TenAnhSP}")
                      ).ToList() 
                    : new List<string>(),
                CacBienThe = listBienThe.Select(b => {
                    var colorObj = allColors.FirstOrDefault(c => c.Id == b.MauId);
                    var sizeObj = allSizes.FirstOrDefault(s => s.Id == b.SizeId);
                    return new BienTheChiTietDto
                    {
                        Id = b.Id,
                        MauId = b.MauId,
                        // Ưu tiên lấy từ variant DTO (đã được nạp qua Include), nếu null thì lấy từ mảng allColors thủ công
                        TenMau = !string.IsNullOrEmpty(b.TenMau) ? b.TenMau : colorObj?.TenMau,
                        MaMau = !string.IsNullOrEmpty(b.MaMau) ? b.MaMau : colorObj?.MaMau,
                        SizeId = b.SizeId,
                        TenSize = !string.IsNullOrEmpty(b.TenSize) ? b.TenSize : sizeObj?.TenSize,
                        SoLuongTon = b.SoLuongTon
                    };
                }).ToList()
            };
        }
    }
}