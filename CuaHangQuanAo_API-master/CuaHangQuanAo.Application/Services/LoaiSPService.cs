using CuaHangQuanAo.Application.DTOs.LoaiSP;
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
    public class LoaiSPService : ILoaiSPService
    {
        private readonly IGenericRepository<LoaiSP> _repository;

        public LoaiSPService(IGenericRepository<LoaiSP> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<LoaiSPDto>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();
            return data.Select(x => new LoaiSPDto { Id = x.Id, TenLSP = x.TenLSP, MoTa = x.MoTa });
        }

        public async Task<LoaiSPDto?> GetByIdAsync(int id)
        {
            var data = await _repository.GetByIdAsync(id);
            if (data == null) return null;
            return new LoaiSPDto { Id = data.Id, TenLSP = data.TenLSP, MoTa = data.MoTa };
        }

        public async Task AddAsync(CreateUpdateLoaiSPDto dto)
        {
            var loaiSp = new LoaiSP { TenLSP = dto.TenLSP, MoTa = dto.MoTa };
            await _repository.AddAsync(loaiSp);
        }

        public async Task UpdateAsync(int id, CreateUpdateLoaiSPDto dto)
        {
            var data = await _repository.GetByIdAsync(id);
            if (data != null)
            {
                data.TenLSP = dto.TenLSP;
                data.MoTa = dto.MoTa;
                await _repository.UpdateAsync(data);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
        public async Task<IEnumerable<LoaiSPDto>> SearchAsync(string keyword)
        {
            var data = await _repository.GetAllAsync();
 
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return data.Select(x => new LoaiSPDto { Id = x.Id, TenLSP = x.TenLSP, MoTa = x.MoTa });
            }
            var result = data.Where(x => x.TenLSP != null && x.TenLSP.ToLower().Contains(keyword.ToLower()));

            return result.Select(x => new LoaiSPDto { Id = x.Id, TenLSP = x.TenLSP, MoTa = x.MoTa });
        }
    }
}
