using LostAndFoundWebApp.Models;
using LostAndFoundWebApp.Services.Mysql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Xml.Linq;

namespace LostAndFoundWebApp.Pages
{
    public class PersonModel : PageModel
    {
        [BindProperty]
        public int UserId { get; set; } = 0;

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public List<Models.Claim> Claims { get; set; } = [];
        public void OnGet()
        {
            int userID;
            try
            {
                var userIDClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                userID = int.Parse(userIDClaim ?? string.Empty);
            }
            catch (Exception)
            {
                ModelState.AddModelError("userID", "用户未登录");
                return;
            }
            var user = DatabaseOperate.GetUserById(userID);
            if (user == null)
            {
                ModelState.AddModelError("user", "用户不存在");
                return;
            }
            UserId = user.UserId;
            Name = user.Name;
            Email = user.Email;
            Claims = DatabaseOperate.GetClaimsUser(userID);
        }
    }
}
