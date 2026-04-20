using CuaHangQuanAo.Application.DTOs.Size;
using CuaHangQuanAo.Application.Interfaces;
using CuaHangQuanAo.Domain.Interfaces;
using CuaHangQuanAo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Services
{
    public class SizeService : ISizeService
    {
        private readonly IGenericRepository<Size> _repository;
        public SizeService(IGenericRepository<Size> repository) { _repository = repository; }

        public async Task<IEnumerable<SizeDto>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync(new string[] { "LoaiSP" });
            return data.Select(x => new SizeDto
            {
                Id = x.Id,
                TenSize = x.TenSize,
                LoaiId = x.LoaiId ?? 0,
                TenLoai = x.LoaiSP?.TenLSP
            });
        }

        public async Task AddAsync(CreateUpdateSizeDto dto)
        {
            await _repository.AddAsync(new Size { TenSize = dto.TenSize, LoaiId = dto.LoaiId });
        }

        public async Task UpdateAsync(int id, CreateUpdateSizeDto dto)
        {
            var s = await _repository.GetByIdAsync(id);
            if (s != null)
            {
                s.TenSize = dto.TenSize;
                s.LoaiId = dto.LoaiId;
                await _repository.UpdateAsync(s);
            }
        }

        public async Task DeleteAsync(int id) { await _repository.DeleteAsync(id); }
    }
}
