using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace LostAndFoundWebApp.Pages
{
    public class VerifyLoginModel(IMemoryCache memoryCache) : PageModel
    {
        private readonly IMemoryCache _memoryCache = memoryCache;
        public async Task<IActionResult> OnGetAsync(string token)
        {
            // 检查缓存中是否存在该令牌，并获取对应的邮箱
            if (_memoryCache.TryGetValue(token, out string? email) && email != null)
            {
                // 删除缓存中的令牌
                _memoryCache.Remove(token);

                // 创建用户身份认证信息
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email), // 将邮箱存储为用户名
            new Claim(ClaimTypes.Email, email) // 可选：显式存储邮箱声明
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = true, // 设置持久化登录
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1) // 设置认证有效期
                };

                // 通过 Cookie 进行用户身份认证
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                // 登录成功后跳转到主页
                return RedirectToPage("/Index");
            }

            // 如果令牌无效或不存在，重定向到登录页面
            return RedirectToPage("/Login");
        }
    }
}
