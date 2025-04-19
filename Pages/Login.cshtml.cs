using LostAndFoundWebApp.Controllers.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebApp.Pages
{
    public class LoginModel(IEmailSender emailSender) : PageModel
    {
        private readonly IEmailSender _emailSender = emailSender;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ���ɵ�¼����
            var loginToken = Guid.NewGuid().ToString();
            var loginLink = Url.Page(
                "/VerifyLogin",
                pageHandler: null,
                values: new { token = loginToken },
                protocol: Request.Scheme);

            // �����ʼ�
            await _emailSender.SendEmailAsync(Email, "��¼����", $"������ɵ�¼��{loginLink}");

            return RedirectToPage("/LoginConfirmation");
        }
    }
}
