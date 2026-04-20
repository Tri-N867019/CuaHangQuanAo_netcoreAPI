using CuaHangQuanAo.Application.DTOs.Size;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangQuanAo.Application.Interfaces
{
    public interface ISizeService
    {
        Task<IEnumerable<SizeDto>> GetAllAsync();
        Task AddAsync(CreateUpdateSizeDto dto);
        Task UpdateAsync(int id, CreateUpdateSizeDto dto);
        Task DeleteAsync(int id);
    }
}
