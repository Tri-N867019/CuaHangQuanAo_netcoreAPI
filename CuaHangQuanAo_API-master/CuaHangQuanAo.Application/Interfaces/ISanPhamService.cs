using CuaHangQuanAo.Application.DTOs.SanPham;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Interfaces
{
    public interface ISanPhamService
    {
        Task<IEnumerable<SanPhamDto>> GetAllAsync(string? searchKeyword = null);
        Task<int> AddAsync(CreateUpdateSanPhamDto dto);
        Task<SanPhamDto?> GetByIdAsync(int id);
        Task UpdateAsync(int id, CreateUpdateSanPhamDto dto);
        Task DeleteAsync(int id);
        Task<SanPhamDetailDto?> GetChiTietSanPhamAsync(int id, string baseUrl);
    }
}
