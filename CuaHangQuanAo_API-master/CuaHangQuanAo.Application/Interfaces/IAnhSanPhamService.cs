using CuaHangQuanAo.Application.DTOs.AnhSanPham;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Interfaces
{
    public interface IAnhSanPhamService
    {
        Task<IEnumerable<AnhSanPhamDto>> GetBySanPhamIdAsync(int sanPhamId, string baseUrl);
        Task AddAsync(CreateUpdateAnhSanPhamDto dto);
        Task UpdateAsync(int id, CreateUpdateAnhSanPhamDto dto);
        Task DeleteAsync(int id);
    }
}
