using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Atles.Server.Services
{
    /// <summary>
    /// Only used for ASP.NET Core Identity
    /// </summary>
    public class MailSender : IEmailSender
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<MailSender> _logger;

        public MailSender(IOptions<MailSettings> mailSettings, ILogger<MailSender> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var message = new MailMessage
                {
                    From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName),
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = htmlMessage
                };

                message.To.Add(new MailAddress(email));

                var smtp = new SmtpClient
                {
                    Port = _mailSettings.Port,
                    Host = _mailSettings.Host,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                await smtp.SendMailAsync(message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }
    }
}
