using CuaHangQuanAo.Application.DTOs.SanPham;
using CuaHangQuanAo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamController : ControllerBase
    {
        private readonly ISanPhamService _sanPhamService;

        public SanPhamController(ISanPhamService sanPhamService)
        {
            _sanPhamService = sanPhamService;
        }

        // Danh sách sản pham (GET)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] string? search = null)
        {
            var data = await _sanPhamService.GetAllAsync(search);
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Create(CreateUpdateSanPhamDto dto)
        {
            var id = await _sanPhamService.AddAsync(dto);
            return Ok(new { message = "Thêm sản phẩm thành công!", id = id });
        }
        
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _sanPhamService.GetByIdAsync(id);
            if (data == null) return NotFound("Không tìm thấy sản phẩm!");
            return Ok(data);
        }

        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Update(int id, CreateUpdateSanPhamDto dto)
        {
            await _sanPhamService.UpdateAsync(id, dto);
            return Ok("Cập nhật thành công!");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin1,admin,nhanvien")]
        public async Task<IActionResult> Delete(int id)
        {
            await _sanPhamService.DeleteAsync(id);
            return Ok("Xóa thành công!");
        }

        [HttpGet("chitiet/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetChiTiet(int id)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var data = await _sanPhamService.GetChiTietSanPhamAsync(id, baseUrl);

            if (data == null) return NotFound("Không tìm thấy sản phẩm này!");

            return Ok(data);
        }
    }
}
