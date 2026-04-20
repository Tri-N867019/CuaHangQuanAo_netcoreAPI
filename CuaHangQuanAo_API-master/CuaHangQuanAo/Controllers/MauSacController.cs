using CuaHangQuanAo.Application.DTOs.MauSac;
using CuaHangQuanAo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class MauSacController : ControllerBase
    {
        private readonly IMauSacService _mauSacService;

        public MauSacController(IMauSacService mauSacService)
        {
            _mauSacService = mauSacService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _mauSacService.GetAllAsync());
        }


        [HttpPost]
        
        public async Task<IActionResult> Create(CreateUpdateMauSacDto dto)
        {
            await _mauSacService.AddAsync(dto);
            return Ok("Thêm màu sắc thành công!");
        }

        [HttpPut("{id}")]
        
        public async Task<IActionResult> Update(int id, CreateUpdateMauSacDto dto)
        {
            await _mauSacService.UpdateAsync(id, dto);
            return Ok("Cập nhật màu sắc thành công!");
        }

        [HttpDelete("{id}")]
        
        public async Task<IActionResult> Delete(int id)
        {
            await _mauSacService.DeleteAsync(id);
            return Ok("Xóa màu sắc thành công!");
        }
    }
}
