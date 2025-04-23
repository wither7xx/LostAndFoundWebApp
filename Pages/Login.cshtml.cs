using LostAndFoundWebApp.Services.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace LostAndFoundWebApp.Pages
{
    public class LoginModel(IEmailSender emailSender, IMemoryCache memoryCache) : PageModel
    {
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IMemoryCache _memoryCache = memoryCache;

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
            // 生成登录令牌
            var loginToken = Guid.NewGuid().ToString();
            // 存储令牌和过期时间
            var tokenExpiry = DateTime.UtcNow.AddMinutes(15);
            _memoryCache.Set(loginToken, Email, tokenExpiry - DateTime.UtcNow);

            // 生成登录链接
            var loginLink = Url.Page(
                "/VerifyLogin",
                pageHandler: null,
                values: new { token = loginToken },
                protocol: Request.Scheme);

            // 发送邮件
            await _emailSender.SendEmailAsync(Email, "登录链接", $"点击即可登录：{loginLink}");

            return RedirectToPage("/LoginConfirmation");
        }
    }
}
