using CuaHangQuanAo.Application.DTOs.AnhSanPham;
using CuaHangQuanAo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnhSanPhamController : ControllerBase
    {
        private readonly IAnhSanPhamService _anhSanPhamService;
        private readonly IWebHostEnvironment _env;

        public AnhSanPhamController(IAnhSanPhamService anhSanPhamService, IWebHostEnvironment env)
        {
            _anhSanPhamService = anhSanPhamService;
            _env = env;
        }

        // Lấy tất cả ảnh của 1 sản phẩm cụ thể
        [HttpGet("sanpham/{sanPhamId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySanPhamId(int sanPhamId)
        {
            // Controller nằm ở API nên gọi Request.Scheme thoải mái không lỗi
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            // Truyền cái baseUrl đó xuống cho Service
            var data = await _anhSanPhamService.GetBySanPhamIdAsync(sanPhamId, baseUrl);

            return Ok(data);
        }

        public class AnhSanPhamUploadRequest
        {
            public IFormFile File { get; set; } = null!;
            public int SanPhamId { get; set; }
        }

        [HttpPost]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Create([FromForm] AnhSanPhamUploadRequest request)
        {
            if (request.File == null || request.File.Length == 0) return BadRequest("File ảnh không hợp lệ");

            string uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + request.File.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(fileStream);
            }

            var dto = new CreateUpdateAnhSanPhamDto
            {
                SanPhamId = request.SanPhamId,
                TenAnhSP = uniqueFileName
            };

            await _anhSanPhamService.AddAsync(dto);
            return Ok("Thêm ảnh thành công!");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Update(int id, CreateUpdateAnhSanPhamDto dto)
        {
            await _anhSanPhamService.UpdateAsync(id, dto);
            return Ok("Cập nhật ảnh thành công!");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin1,Admin,Nhanvien")]
        public async Task<IActionResult> Delete(int id)
        {
            await _anhSanPhamService.DeleteAsync(id);
            return Ok("Xóa ảnh thành công!");
        }
    }
}
