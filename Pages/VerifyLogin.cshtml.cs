using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using LostAndFoundWebApp.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using LostAndFoundWebApp.Services.Mysql;

namespace LostAndFoundWebApp.Pages
{
    public class VerifyLoginModel(IMemoryCache memoryCache) : PageModel
    {
        private readonly IMemoryCache _memoryCache = memoryCache;
        public async Task<IActionResult> OnGetAsync(string token)
        {
            if (_memoryCache.TryGetValue(token, out string? email) && email != null)
            {
                // 删除缓存中的令牌
                _memoryCache.Remove(token);

                var user = DatabaseOperate.GetUserByEmail(email);

                if (user == null)
                {
                    int index = email.IndexOf('@');
                    string username;
                    if (index > 0)
                    {
                        username = email[..index]; // 截取从下标为0到'@'的位置作为用户名
                    }
                    else
                    {
                        username = email; // 如果没有'@'，则直接使用email作为用户名
                    }
                    // 如果用户不存在，则创建新用户
                    DatabaseOperate.CreateUser(new()
                    {
                        Email = email,
                        Password = UserMetadata.Password.PasswordPlaceholder,
                        Name = username,
                        Role = UserMetadata.Role.User,
                        IsValid = true
                    });
                    user = DatabaseOperate.GetUserByEmail(email);
                }

                if (user != null)
                {

                    // 创建用户会话
                    List<System.Security.Claims.Claim> claims = [
                        new(ClaimTypes.Name, user.Name),
                        new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new(ClaimTypes.Email, email),
                        new(ClaimTypes.Role, user.Role)
                    ];
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToPage("/Index");
                }
            }

            return RedirectToPage("/Login");
        }
    }
}
