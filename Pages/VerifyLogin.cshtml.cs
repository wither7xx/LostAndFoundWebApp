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
        public IActionResult OnGet(string token)
        {
            if (_memoryCache.TryGetValue(token, out string? email) && email != null)
            {
                // 删除缓存中的令牌
                _memoryCache.Remove(token);

                // TODO:创建用户会话

                return RedirectToPage("/Index");
            }

            return RedirectToPage("/Login");
        }
    }
}
