using CuaHangQuanAo.Application.DTOs.ThuongHieu;
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
    public class ThuongHieuService : IThuongHieuService
    {
        private readonly IGenericRepository<ThuongHieu> _repository;

        public ThuongHieuService(IGenericRepository<ThuongHieu> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ThuongHieuDto>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();
            return data.Select(x => new ThuongHieuDto
            {
                Id = x.Id,
                TenTH = x.TenTH,
                MoTa = x.MoTa,
                NgayTao = x.NgayTao
            });
        }

        public async Task<ThuongHieuDto?> GetByIdAsync(int id)
        {
            var data = await _repository.GetByIdAsync(id);
            if (data == null) return null;
            return new ThuongHieuDto { Id = data.Id, TenTH = data.TenTH, MoTa = data.MoTa, NgayTao = data.NgayTao };
        }

        public async Task AddAsync(CreateUpdateThuongHieuDto dto)
        {
            var thuongHieu = new ThuongHieu
            {
                TenTH = dto.TenTH,
                MoTa = dto.MoTa,
                NgayTao = DateTime.Now // Tự động lấy giờ hiện tại của hệ thống gán vào
            };
            await _repository.AddAsync(thuongHieu);
        }

        public async Task UpdateAsync(int id, CreateUpdateThuongHieuDto dto)
        {
            var data = await _repository.GetByIdAsync(id);
            if (data != null)
            {
                data.TenTH = dto.TenTH;
                data.MoTa = dto.MoTa;
                // Khi cập nhật thì thường không sửa NgayTao nên mình chỉ cập nhật Tên
                await _repository.UpdateAsync(data);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
