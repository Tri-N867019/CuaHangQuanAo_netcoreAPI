using CuaHangQuanAo.Application.DTOs.LoaiSP;
using CuaHangQuanAo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiSPController : ControllerBase
    {
        private readonly ILoaiSPService _loaiSPService;

        public LoaiSPController(ILoaiSPService loaiSPService)
        {
            _loaiSPService = loaiSPService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _loaiSPService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _loaiSPService.GetByIdAsync(id);
            if (data == null) return NotFound("Không tìm thấy!");
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Create(CreateUpdateLoaiSPDto dto)
        {
            await _loaiSPService.AddAsync(dto);
            return Ok("Thêm thành công!");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Update(int id, CreateUpdateLoaiSPDto dto)
        {
            await _loaiSPService.UpdateAsync(id, dto);
            return Ok("Cập nhật thành công!");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Delete(int id)
        {
            await _loaiSPService.DeleteAsync(id);
            return Ok("Xóa thành công!");
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var data = await _loaiSPService.SearchAsync(keyword);
            return Ok(data);
        }
    }
}
