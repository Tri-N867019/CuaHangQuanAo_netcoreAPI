using CuaHangQuanAo.Application.DTOs.ThuongHieu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Interfaces
{
    public interface IThuongHieuService
    {
        Task<IEnumerable<ThuongHieuDto>> GetAllAsync();
        Task<ThuongHieuDto?> GetByIdAsync(int id);
        Task AddAsync(CreateUpdateThuongHieuDto dto);
        Task UpdateAsync(int id, CreateUpdateThuongHieuDto dto);
        Task DeleteAsync(int id);
    }
}
