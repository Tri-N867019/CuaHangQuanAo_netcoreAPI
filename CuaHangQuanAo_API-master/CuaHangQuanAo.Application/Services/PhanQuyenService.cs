using CuaHangQuanAo.Application.DTOs.PhanQuyen;
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
    public class PhanQuyenService : IPhanQuyenService
    {
        private readonly IGenericRepository<PhanQuyen> _repository;

        public PhanQuyenService(IGenericRepository<PhanQuyen> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PhanQuyenDto>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();
            return data.Select(x => new PhanQuyenDto
            {
                Id = x.Id,
                TenQuyen = x.TenQuyen,
                MoTa = x.MoTa
            });
        }

        public async Task<PhanQuyenDto?> GetByIdAsync(int id)
        {
            var data = await _repository.GetByIdAsync(id);
            if (data == null) return null;
            return new PhanQuyenDto { Id = data.Id, TenQuyen = data.TenQuyen, MoTa = data.MoTa };
        }

        public async Task AddAsync(CreateUpdatePhanQuyenDto dto)
        {
            var phanQuyen = new PhanQuyen { TenQuyen = dto.TenQuyen, MoTa = dto.MoTa };
            await _repository.AddAsync(phanQuyen);
        }

        public async Task UpdateAsync(int id, CreateUpdatePhanQuyenDto dto)
        {
            var data = await _repository.GetByIdAsync(id);
            if (data != null)
            {
                data.TenQuyen = dto.TenQuyen;
                data.MoTa = dto.MoTa;
                await _repository.UpdateAsync(data);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
