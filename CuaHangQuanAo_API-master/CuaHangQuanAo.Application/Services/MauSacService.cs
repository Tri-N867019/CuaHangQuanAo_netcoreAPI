using CuaHangQuanAo.Application.DTOs.MauSac;
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
    public class MauSacService : IMauSacService
    {
        private readonly IGenericRepository<MauSac> _repository;
        public MauSacService(IGenericRepository<MauSac> repository) { _repository = repository; }

        public async Task<IEnumerable<MauSacDto>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync(new string[] { "LoaiSP" });
            return data.Select(x => new MauSacDto
            {
                Id = x.Id,
                TenMau = x.TenMau,
                MaMau = x.MaMau,
                LoaiId = x.LoaiId ?? 0,
                TenLoai = x.LoaiSP?.TenLSP
            });
        }

        public async Task AddAsync(CreateUpdateMauSacDto dto)
        {
            await _repository.AddAsync(new MauSac { 
                TenMau = dto.TenMau,
                MaMau = dto.MaMau, 
                LoaiId = dto.LoaiId 
            });
        }

        public async Task UpdateAsync(int id, CreateUpdateMauSacDto dto)
        {
            var ms = await _repository.GetByIdAsync(id);
            if (ms != null)
            {
                ms.TenMau = dto.TenMau;
                ms.MaMau = dto.MaMau;
                ms.LoaiId = dto.LoaiId;
                await _repository.UpdateAsync(ms);
            }
        }

        public async Task DeleteAsync(int id) 
        { 
            await _repository.DeleteAsync(id); 
        }

        public async Task<MauSacDto?> GetByIdAsync(int id)
        {
            // Lấy tất cả và kèm theo LoaiSP
            var data = await _repository.GetAllAsync(new string[] { "LoaiSP" });
            var ms = data.FirstOrDefault(x => x.Id == id);

            if (ms == null) return null;

            return new MauSacDto
            {
                Id = ms.Id,
                TenMau = ms.TenMau,
                MaMau = ms.MaMau,
                LoaiId = ms.LoaiId ?? 0,
                TenLoai = ms.LoaiSP?.TenLSP
            };
        }
    }
}
