using CuaHangQuanAo.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CuaHangQuanAo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnpayService _vnpayService;
        private readonly IHoaDonService _hoaDonService;
        private readonly IConfiguration _configuration;

        public PaymentController(IVnpayService vnpayService, IHoaDonService hoaDonService, IConfiguration configuration)
        {
            _vnpayService = vnpayService;
            _hoaDonService = hoaDonService;
            _configuration = configuration;
        }

        [HttpGet("create-vnpay-url/{hoaDonId}")]
        public async Task<IActionResult> CreateVnpayUrl(int hoaDonId)
        {
            var hoaDon = await _hoaDonService.GetOrderByIdAsync(hoaDonId);
            if (hoaDon == null) return NotFound("Không tìm thấy đơn hàng!");

            var tongTien = (hoaDon.TongTien ?? 0) + (hoaDon.PhiVanChuyen ?? 0);
            var url = _vnpayService.CreatePaymentUrl(HttpContext, hoaDonId, tongTien);

            return Ok(new { url });
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VnpayReturn()
        {
            if (Request.Query.Count > 0)
            {
                var isValid = _vnpayService.ValidateSignature(Request.Query);

                if (isValid)
                {
                    var vnp_ResponseCode = Request.Query["vnp_ResponseCode"];
                    var vnp_TransactionStatus = Request.Query["vnp_TransactionStatus"];
                    var vnp_TxnRef = Request.Query["vnp_TxnRef"];
                    var hoaDonId = int.Parse(vnp_TxnRef!);

                    var frontendUrl = _configuration["VnpaySettings:FrontendBaseUrl"];

                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        // Thanh toán thành công -> Cập nhật trạng thái hóa đơn
                        await _hoaDonService.UpdateOrderStatusAsync(hoaDonId, 1); // Giả sử 1 là "Đã Thanh Toán" hoặc "Đã xác nhận"
                        
                        return Redirect($"{frontendUrl}/vnpay-return.html?status=success&orderId={vnp_TxnRef}");
                    }
                    else
                    {
                        return Redirect($"{frontendUrl}/vnpay-return.html?status=fail&code={vnp_ResponseCode}");
                    }
                }
            }
            var fallbackUrl = _configuration["VnpaySettings:FrontendBaseUrl"];
            return Redirect($"{fallbackUrl}/vnpay-return.html?status=error");
        }

        [HttpGet("vnpay-ipn")]
        public async Task<IActionResult> VnpayIpn()
        {
             var isValid = _vnpayService.ValidateSignature(Request.Query);
             if (isValid)
             {
                 var vnp_ResponseCode = Request.Query["vnp_ResponseCode"];
                 var vnp_TransactionStatus = Request.Query["vnp_TransactionStatus"];
                 var vnp_TxnRef = Request.Query["vnp_TxnRef"];
                 var hoaDonId = int.Parse(vnp_TxnRef!);

                 if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                 {
                     await _hoaDonService.UpdateOrderStatusAsync(hoaDonId, 1);
                     return Ok(new { RspCode = "00", Message = "Confirm Success" });
                 }
             }
             return Ok(new { RspCode = "97", Message = "Invalid Signature" });
        }
    }
}
