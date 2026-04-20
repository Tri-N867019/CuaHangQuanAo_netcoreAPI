using CuaHangQuanAo.Application.DTOs.PhanQuyen;
using CuaHangQuanAo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin1,Admin")]
    public class PhanQuyenController : ControllerBase
    {
        private readonly IPhanQuyenService _service;

        public PhanQuyenController(IPhanQuyenService service)
        {
            _service = service;
        }

        [HttpGet]
       
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        } 

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin1,Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound("Không tìm thấy quyền!");
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin1,Admin")]
        public async Task<IActionResult> Create(CreateUpdatePhanQuyenDto dto)
        {
            await _service.AddAsync(dto);
            return Ok("Thêm quyền thành công!");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin1,Admin")]
        public async Task<IActionResult> Update(int id, CreateUpdatePhanQuyenDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok("Cập nhật quyền thành công!");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin1,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok("Xóa quyền thành công!");
        }
    }
}
