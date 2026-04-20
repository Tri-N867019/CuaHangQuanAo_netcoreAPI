using CuaHangQuanAo.Application.DTOs.SanPhamBienThe;
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
    public class SanPhamBienTheService : ISanPhamBienTheService
    {
        private readonly IGenericRepository<SanPhamBienThe> _repository;

        public SanPhamBienTheService(IGenericRepository<SanPhamBienThe> repository)
        {
            _repository = repository;
        }

        // Lấy tất cả biến thể của 1 sản phẩm cụ thể
        public async Task<IEnumerable<SanPhamBienTheDto>> GetBySanPhamIdAsync(int sanPhamId)
        {
            var data = await _repository.GetAllAsync(new string[] { "MauSac", "Size" });

            return data.Where(x => x.SanPhamId == sanPhamId)
                       .Select(x => new SanPhamBienTheDto
                       {
                           Id = x.Id,
                           SanPhamId = x.SanPhamId ?? 0,
                           MauId = x.MauId ?? 0,
                           SizeId = x.SizeId ?? 0,
                           SoLuongTon = x.SoLuongTon ?? 0,
                           MaMau = x.MauSac?.MaMau,
                           TenMau = x.MauSac?.TenMau,
                           TenSize = x.Size?.TenSize
                       });
        }

        public async Task<SanPhamBienTheDto?> GetByIdAsync(int id)
        {
            // Lấy tất cả và kèm theo MauSac, Size
            var data = await _repository.GetAllAsync(new string[] { "MauSac", "Size" });
            var x = data.FirstOrDefault(v => v.Id == id);
            if (x == null) return null;

            return new SanPhamBienTheDto
            {
                Id = x.Id,
                SanPhamId = x.SanPhamId ?? 0,
                MauId = x.MauId ?? 0,
                SizeId = x.SizeId ?? 0,
                SoLuongTon = x.SoLuongTon ?? 0,
                MaMau = x.MauSac?.MaMau,
                TenMau = x.MauSac?.TenMau,
                TenSize = x.Size?.TenSize
            };
        }

        public async Task AddAsync(CreateUpdateSanPhamBienTheDto dto)
        {
            var entity = new SanPhamBienThe
            {
                SanPhamId = dto.SanPhamId,
                MauId = dto.MauId,
                SizeId = dto.SizeId,
                SoLuongTon = dto.SoLuongTon
            };
            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(int id, CreateUpdateSanPhamBienTheDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                entity.MauId = dto.MauId;
                entity.SizeId = dto.SizeId;
                entity.SoLuongTon = dto.SoLuongTon;
                await _repository.UpdateAsync(entity);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
