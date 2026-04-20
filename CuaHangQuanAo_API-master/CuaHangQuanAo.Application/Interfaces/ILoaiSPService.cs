using CuaHangQuanAo.Application.DTOs.LoaiSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Interfaces
{
    public interface ILoaiSPService
    {
        Task<IEnumerable<LoaiSPDto>> GetAllAsync();
        Task<LoaiSPDto?> GetByIdAsync(int id);
        Task AddAsync(CreateUpdateLoaiSPDto dto);
        Task UpdateAsync(int id, CreateUpdateLoaiSPDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<LoaiSPDto>> SearchAsync(string keyword);
    }
}
