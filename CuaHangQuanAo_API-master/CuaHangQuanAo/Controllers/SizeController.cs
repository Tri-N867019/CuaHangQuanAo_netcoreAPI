using CuaHangQuanAo.Application.DTOs.Size;
using CuaHangQuanAo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizeController : ControllerBase
    {
        private readonly ISizeService _sizeService;

        public SizeController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _sizeService.GetAllAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Create(CreateUpdateSizeDto dto)
        {
            await _sizeService.AddAsync(dto);
            return Ok("Thêm kích cỡ thành công!");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Update(int id, CreateUpdateSizeDto dto)
        {
            await _sizeService.UpdateAsync(id, dto);
            return Ok("Cập nhật kích cỡ thành công!");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Delete(int id)
        {
            await _sizeService.DeleteAsync(id);
            return Ok("Xóa kích cỡ thành công!");
        }
    }
}
