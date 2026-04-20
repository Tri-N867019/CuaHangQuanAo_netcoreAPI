using CuaHangQuanAo.Application.Interfaces;
using CuaHangQuanAo.Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;

namespace CuaHangQuanAo.Infrastructure.Services
{
    public class VnpayService : IVnpayService
    {
        private readonly IConfiguration _configuration;

        public VnpayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(HttpContext httpContext, int hoaDonId, decimal tongTien)
        {
            var vnpay = new VnpayLibrary();
            var vnp_TmnCode = _configuration["VnpaySettings:vnp_TmnCode"];
            var vnp_HashSecret = _configuration["VnpaySettings:vnp_HashSecret"];
            var vnp_Url = _configuration["VnpaySettings:vnp_Url"];
            var vnp_ReturnUrl = _configuration["VnpaySettings:vnp_ReturnUrl"];

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(tongTien * 100)).ToString()); // VNPAY amount is in cents (VND * 100)
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", GetIpAddress(httpContext));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan hoa don #" + hoaDonId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", hoaDonId.ToString());

            var paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return paymentUrl;
        }

        public bool ValidateSignature(IQueryCollection collections)
        {
            var vnpay = new VnpayLibrary();
            var vnp_HashSecret = _configuration["VnpaySettings:vnp_HashSecret"];

            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value);
                }
            }

            var vnp_SecureHash = collections["vnp_SecureHash"];
            return vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
        }

        private string GetIpAddress(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }
            return ipAddress;
        }
    }
}
