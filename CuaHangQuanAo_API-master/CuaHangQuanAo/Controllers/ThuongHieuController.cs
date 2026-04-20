using CuaHangQuanAo.Application.DTOs.ThuongHieu;
using CuaHangQuanAo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class ThuongHieuController : ControllerBase
    {
        private readonly IThuongHieuService _thuongHieuService;

        public ThuongHieuController(IThuongHieuService thuongHieuService)
        {
            _thuongHieuService = thuongHieuService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _thuongHieuService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _thuongHieuService.GetByIdAsync(id);
            if (data == null) return NotFound("Không tìm thấy thương hiệu này!");
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Create(CreateUpdateThuongHieuDto dto)
        {
            await _thuongHieuService.AddAsync(dto);
            return Ok("Thêm thương hiệu thành công!");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Update(int id, CreateUpdateThuongHieuDto dto)
        {
            await _thuongHieuService.UpdateAsync(id, dto);
            return Ok("Cập nhật thành công!");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Delete(int id)
        {
            await _thuongHieuService.DeleteAsync(id);
            return Ok("Xóa thành công!");
        }
    }
}
