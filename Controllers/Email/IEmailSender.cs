using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LostAndFoundWebApp.Controllers.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string targetEmail, string subject, string message);
    }
}
