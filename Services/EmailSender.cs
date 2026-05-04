using MimeKit;
using MailKit.Net.Smtp;
using EmployeeManagementSystem.Models.Dtos;

using System.Net;

namespace EmployeeManagementSystem.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string Host;
        private readonly int Port;
        private readonly string Email;
        private readonly string Password;
        private readonly ILogger<EmailSender> _logger;


        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            Host = configuration["EmailSettings:Host"] ?? "";
            Email = configuration["EmailSettings:Email"] ?? "";
            Password = configuration["EmailSettings:Password"] ?? "";
            Port = int.Parse(configuration["EmailSettings:Port"] ?? "587");

            _logger = logger;
        }

        public async Task SendPasswordResetAsync(string email, string link)
                {
                try
                {
                 var message = new MimeMessage();

                 message.From.Add(new MailboxAddress("System", Email));
                 message.To.Add(new MailboxAddress("", email));
                 message.Subject = "Reset Your Password";

                 message.Body = new TextPart("html")
                {
                  Text = $"Click <a href='{link}'>here</a> to reset your password."
                };

                await SendAsync(message);
                }
                 catch (Exception ex)
                  {
                    _logger.LogError(ex, "Password reset email failed");
                  }
                }
        public async Task SendLoginAlertAsync(LoginAlertEmailDto dto)
        {
                var message = new MimeMessage();
                var safeDevice = WebUtility.HtmlEncode(dto.Device);
                var safeIp = WebUtility.HtmlEncode(dto.Ip);

                message.From.Add(new MailboxAddress("System", Email));
                message.To.Add(new MailboxAddress("", dto.ToEmail));
                message.Subject = "Login Alert";

                message.Body = new TextPart("html")
               {
                 Text = $@"
                <div style='font-family: Arial; padding:20px; background:#f4f4f4'>
                <div style='background:white; padding:20px; border-radius:10px'>
                <h2 style='color:#333'>Login Alert!!</h2>
                <p>A new login was detected on your account.</p>

                <table style='width:100%; border-collapse:collapse'>
                <tr>
                <td><b>Device</b></td>
                <td>{safeDevice}</td>
                </tr>
                <tr>
                <td><b>IP Address</b></td>
                <td>{safeIp}</td>
                </tr>
                <tr>
                <td><b>Time</b></td>
                <td>{dto.Time.ToString("f")}</td>
                </tr>
                </table>

                <p style='margin-top:20px; color:red'>
                If this wasn't you, please reset your password immediately.
                </p>
                <p style='font-size:12px;color:gray'>
                If you recognize this activity, you can safely ignore this email.
                </p>
                </div>
                </div>
                "
                };
                 int retryCount = 0;

                while (retryCount < 3)
                {
                    try
                {
                 await SendAsync(message);
                 return;
                }
                 catch (Exception ex)
                {
                 retryCount++;
                 _logger.LogWarning(ex, "Retry {RetryCount} failed", retryCount);
                 await Task.Delay(2000);
                }
              }

                 _logger.LogError("Failed to send login alert after retries");
        }

            
        
        
                private async Task SendAsync(MimeMessage message)
                {
                using var client = new SmtpClient();
                

                await client.ConnectAsync(Host, Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(Email, Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                
                }
    }
}
