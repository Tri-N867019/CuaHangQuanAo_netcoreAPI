using CuaHangQuanAo.Application.DTOs.SanPhamBienThe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Interfaces
{
    public interface ISanPhamBienTheService
    {
        Task<IEnumerable<SanPhamBienTheDto>> GetBySanPhamIdAsync(int sanPhamId);
        Task<SanPhamBienTheDto?> GetByIdAsync(int id);
        Task AddAsync(CreateUpdateSanPhamBienTheDto dto);
        Task UpdateAsync(int id, CreateUpdateSanPhamBienTheDto dto);
        Task DeleteAsync(int id);
    }
}
