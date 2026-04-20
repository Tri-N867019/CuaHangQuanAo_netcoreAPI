using CuaHangQuanAo.Application.DTOs.User;
using CuaHangQuanAo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPut("cap-quyen/{userId}")]
        [Authorize(Roles = "Admin1,Admin")]
        public async Task<IActionResult> CapQuyen(int userId, [FromBody] RoleUpdateDto dto)
        {
            var userIdDangThaoTacStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int adminDangThaoTacId = int.Parse(userIdDangThaoTacStr ?? "0");

            // Chặn việc tự đổi quyền của chính mình
            if (userId == adminDangThaoTacId)
            {
                return BadRequest("Bạn không được phép tự thay đổi quyền của chính mình để tránh bị khóa tài khoản!");
            }

            try
            {
                var thanhCong = await _userService.CapQuyenAsync(adminDangThaoTacId, userId, dto.PhanQuyenId);
                if (!thanhCong) return NotFound("Không tìm thấy tài khoản này!");

                return Ok("Nâng/Hạ quyền thành công!");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                // Trả về thông tin InnerException (lỗi thực từ DB) để gỡ lỗi
                var errorMsg = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"Lỗi hệ thống: {errorMsg}\nChi tiết: {ex.StackTrace}");
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized("Không xác định được danh tính.");
            }

            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound("Tài khoản không tồn tại.");

            return Ok(user);
        }

        [HttpPut("cap-nhat-ho-so")]
        [Authorize]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized("Không xác định được danh tính.");
            }

            var result = await _userService.UpdateProfileAsync(userId, dto);
            if (!result) return NotFound("Tài khoản không tồn tại.");

            return Ok(new { message = "Cập nhật thông tin thành công!" });
        }

        [HttpPost("upload-avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized("Không xác định được danh tính.");
            }

            if (file == null || file.Length == 0) return BadRequest("Vui lòng chọn ảnh.");

            // Lưu file vào thư mục wwwroot/avatars
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            var fileName = $"avatar_{userId}_{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var avatarUrl = $"/avatars/{fileName}";
            var result = await _userService.UpdateAvatarAsync(userId, avatarUrl);

            if (!result) return NotFound("Không tìm thấy người dùng.");

            return Ok(new { message = "Cập nhật ảnh đại diện thành công!", url = avatarUrl });
        }

        [HttpPut("doi-mat-khau")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized("Không xác định được danh tính.");
            }

            try
            {
                var result = await _userService.ChangePasswordAsync(userId, dto);
                if (!result) return NotFound("Tài khoản không tồn tại.");
                return Ok(new { message = "Đổi mật khẩu thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin1,Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }
    }
}
