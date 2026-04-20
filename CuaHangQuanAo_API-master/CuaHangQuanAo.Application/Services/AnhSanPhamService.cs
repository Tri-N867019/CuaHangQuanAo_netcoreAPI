using CuaHangQuanAo.Application.DTOs.AnhSanPham;
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
    public class AnhSanPhamService : IAnhSanPhamService
    {
        private readonly IGenericRepository<AnhSanPham> _repository;

        // Xóa sạch các biến HttpContext rườm rà đi, chỉ giữ lại Repository
        public AnhSanPhamService(IGenericRepository<AnhSanPham> repository)
        {
            _repository = repository;
        }

        // Nhận thêm cái baseUrl từ Controller truyền xuống
        public async Task<IEnumerable<AnhSanPhamDto>> GetBySanPhamIdAsync(int sanPhamId, string baseUrl)
        {
            var data = await _repository.GetAllAsync();

            return data.Where(x => x.SanPhamId == sanPhamId)
                       .Select(x => new AnhSanPhamDto
                       {
                           Id = x.Id,
                           SanPhamId = x.SanPhamId ?? 0,
                           // Ghép cái baseUrl vào tên ảnh nếu k phải là URL tuyệt đối
                           TenAnhSP = x.TenAnhSP!.StartsWith("http") 
                                ? x.TenAnhSP 
                                : (x.TenAnhSP.StartsWith("/") ? $"{baseUrl}{x.TenAnhSP}" : $"{baseUrl}/uploads/{x.TenAnhSP}")
                       });
        }

        public async Task AddAsync(CreateUpdateAnhSanPhamDto dto)
        {
            var entity = new AnhSanPham { SanPhamId = dto.SanPhamId, TenAnhSP = dto.TenAnhSP };
            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(int id, CreateUpdateAnhSanPhamDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                entity.TenAnhSP = dto.TenAnhSP;
                await _repository.UpdateAsync(entity);
            }
        }

        public async Task DeleteAsync(int id) { await _repository.DeleteAsync(id); }
    }
}

