using CuaHangQuanAo.Application.DTOs.MauSac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Interfaces
{
    public interface IMauSacService
    {
        Task<IEnumerable<MauSacDto>> GetAllAsync();
        Task AddAsync(CreateUpdateMauSacDto dto);
        Task UpdateAsync(int id, CreateUpdateMauSacDto dto);
        Task DeleteAsync(int id);
        Task<MauSacDto?> GetByIdAsync(int id);
    }
}
