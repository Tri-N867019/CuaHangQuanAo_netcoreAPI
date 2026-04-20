using CuaHangQuanAo.Application.DTOs.PhanQuyen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Interfaces
{
    public interface IPhanQuyenService
    {
        Task<IEnumerable<PhanQuyenDto>> GetAllAsync();
        Task<PhanQuyenDto?> GetByIdAsync(int id);
        Task AddAsync(CreateUpdatePhanQuyenDto dto);
        Task UpdateAsync(int id, CreateUpdatePhanQuyenDto dto);
        Task DeleteAsync(int id);
    }
}
 