using CuaHangQuanAo.Application.DTOs.User;
using CuaHangQuanAo.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        //  tạo tài khoản
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(CreateUserDto dto)
        {
            try 
            {
                await _userService.AddAsync(dto);
                return Ok(new { message = "Đăng ký tài khoản thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 2. ĐĂNG NHẬP: Kiểm tra mật khẩu và phát vé JWT
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _userService.LoginAsync(dto);

            if (token == null)
            {
                return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác!" });
            }

            // Trả về chuỗi Token để người dùng dán vào Header/Swagger
            return Ok(new
            {
                token = token,
                message = "Đăng nhập thành công!"
            });
        }
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Lấy UserId từ vé JWT
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var user = await _userService.GetByIdAsync(int.Parse(userIdStr));
            if (user == null) return NotFound(new { message = "Không tìm thấy người dùng!" });

            return Ok(user);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromQuery] string email)
        {
            var result = await _userService.ForgotPasswordAsync(email);

            if (!result)
            {
                // Trả về lỗi nếu không tìm thấy email trong hệ thống
                return NotFound(new { message = "Email này chưa được đăng ký trong hệ thống!" });
            }

            return Ok(new { message = "Mật khẩu mới đã được đặt lại thành công. Vui lòng kiểm tra email của bạn!" });
        }
    }
}

