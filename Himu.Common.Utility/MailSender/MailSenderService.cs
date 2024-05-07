using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Himu.Common.Service
{
    public class MailSenderService : IMailSenderService
    {
        private readonly MailSenderOptions _options;
        private readonly ILogger<MailSenderService> _logger;

        public MailSenderService(
            IOptionsSnapshot<MailSenderOptions> optionsSnapshot,
            ILogger<MailSenderService> logger)
        {
            _options = optionsSnapshot.Value;
            _logger = logger;
        }

        public bool Send(string mailTo, string subject, string body, bool useHtml)
        {
            SmtpClient client = new(_options.StmpServer, _options.Port)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_options.SenderMail, _options.Password)
            };

            MailMessage mailMessage = new(_options.SenderMail, mailTo)
            {
                Subject = subject,
                Body = body,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = useHtml,
                Priority = MailPriority.Normal
            };

            try
            {
                client.Send(mailMessage);
                _logger.LogInformation("The email address has been successfully sent to {mailTo}", mailTo);
                return true;
            }
            catch (SmtpException ex)
            {
                _logger.LogError("Error at MailSenderService: {ex}", ex);
                return false;
            }
        }
    }
}