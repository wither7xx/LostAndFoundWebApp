using System.Net.Mail;
using System.Net;
using LostAndFoundWebApp.Controllers.Email;

namespace LostAndFoundWebApp.Controllers.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _sourceEmail;

        public EmailSender(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(configuration["EmailSettings:Email"]);
            ArgumentNullException.ThrowIfNull(configuration["EmailSettings:Password"]);
            ArgumentNullException.ThrowIfNull(configuration["EmailSettings:SmtpServer"]);
            ArgumentNullException.ThrowIfNull(configuration["EmailSettings:SmtpPort"]);
            ArgumentNullException.ThrowIfNull(configuration["EmailSettings:EnableSsl"]);

            _sourceEmail = configuration["EmailSettings:Email"]!;
            var password = configuration["EmailSettings:Password"];
            var smtpServer = configuration["EmailSettings:SmtpServer"];
            var smtpPortString = configuration["EmailSettings:SmtpPort"];
            var enableSslString = configuration["EmailSettings:EnableSsl"];

            if (!int.TryParse(smtpPortString, out var smtpPort))
            {
                throw new FormatException("EmailSettings:SmtpPort must be a valid integer.");
            }

            if (!bool.TryParse(enableSslString, out var enableSsl))
            {
                throw new FormatException("EmailSettings:EnableSsl must be a valid boolean.");
            }

            _smtpClient = new SmtpClient(smtpServer)
            {
                Credentials = new NetworkCredential(_sourceEmail, password),
                Port = smtpPort,
                EnableSsl = enableSsl
            };
        }

        public async Task SendEmailAsync(string targetEmail, string subject, string message)
        {
            var mailMessage = new MailMessage(_sourceEmail, targetEmail, subject, message);
            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
