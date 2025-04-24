using LostAndFoundWebApp.Services.Email;
using LostAndFoundWebApp.Services.Mysql;
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

            // 检查邮箱对应的用户是否有效
            var user = DatabaseOperate.GetUserByEmail(Email);
            if (user != null && !(user.IsValid ?? false))
            {
                TempData["ErrorMessage"] = "该账号已失效或已被封禁。";
                return RedirectToPage("/Index");
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
            try
            {
                await _emailSender.SendEmailAsync(Email, "登录链接", $"点击即可登录：{loginLink}");
            }
            catch (Exception)
            {
                return BadRequest("邮件发送失败");
            }

            return RedirectToPage("/LoginConfirmation");
        }
    }
}
