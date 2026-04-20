using CuaHangQuanAo.Application.DTOs.SanPhamBienThe;
using CuaHangQuanAo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SanPhamBienTheController : ControllerBase
    {
        private readonly ISanPhamBienTheService _sanPhamBienTheService;

        public SanPhamBienTheController(ISanPhamBienTheService sanPhamBienTheService)
        {
            _sanPhamBienTheService = sanPhamBienTheService;
        }

        
        [HttpGet("sanpham/{sanPhamId}")]
        [Authorize(Roles = "Admin1,Admin")]
        public async Task<IActionResult> GetBySanPhamId(int sanPhamId)
        {
            var data = await _sanPhamBienTheService.GetBySanPhamIdAsync(sanPhamId);
            return Ok(data);
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Create(CreateUpdateSanPhamBienTheDto dto)
        {
            await _sanPhamBienTheService.AddAsync(dto);
            return Ok("Thêm phân loại hàng (nhập kho) thành công!");
        }

        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Update(int id, CreateUpdateSanPhamBienTheDto dto)
        {
            await _sanPhamBienTheService.UpdateAsync(id, dto);
            return Ok("Cập nhật phân loại hàng thành công!");
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Delete(int id)
        {
            await _sanPhamBienTheService.DeleteAsync(id);
            return Ok("Xóa phân loại hàng thành công!");
        }
    }
}
