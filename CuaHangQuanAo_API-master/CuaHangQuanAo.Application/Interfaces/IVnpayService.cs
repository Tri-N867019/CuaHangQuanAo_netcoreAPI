using Microsoft.AspNetCore.Http;

namespace CuaHangQuanAo.Application.Interfaces
{
    public interface IVnpayService
    {
        string CreatePaymentUrl(HttpContext httpContext, int hoaDonId, decimal tongTien);
        bool ValidateSignature(IQueryCollection collections);
    }
}
