using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebApp.Pages
{
    public class VerifyLoginModel : PageModel
    {
        public IActionResult OnGet(string token)
        {
            // 验证 token（实验项目中可以简单存储 token 或直接通过 URL 验证）
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("登录链接无效");
            }

            // 模拟登录成功
            return Content("登录成功！");
        }
    }
}
