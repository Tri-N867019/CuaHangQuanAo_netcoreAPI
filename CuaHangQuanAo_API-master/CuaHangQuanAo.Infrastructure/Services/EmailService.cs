using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using CuaHangQuanAo.Application.Interfaces;

namespace CuaHangQuanAo.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailSettings["EmailDisplayName"], emailSettings["EmailUsername"]));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(emailSettings["EmailHost"], int.Parse(emailSettings["EmailPort"]), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(emailSettings["EmailUsername"], emailSettings["EmailPassword"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc ném ngoại lệ để UserService xử lý
                throw new Exception($"Lỗi khi gửi email: {ex.Message}");
            }
        }
    }
}
