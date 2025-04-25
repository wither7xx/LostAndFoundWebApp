using LostAndFoundWebApp.Models;
using LostAndFoundWebApp.Services.Mysql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebApp.Pages.Admin
{
    public class ManageUsersModel : PageModel
    {
        public List<User> Users { get; set; } = [];

        public void OnGet()
        {
            Users = DatabaseOperate.GetAllUsers();
        }

        public IActionResult OnPostToggleState([FromBody] ToggleRequest data)
        {
            if (data == null || data.UserId == null)
            {
                return BadRequest("无效的请求数据");
            }

            int userId = data.UserId.Value;
            var user = DatabaseOperate.GetUserById(userId);
            if (user != null)
            {
                user.IsValid = !(user.IsValid ?? false);
                DatabaseOperate.UpdateUser(user);

                // 返回更新后的状态
                return new JsonResult(new { isValid = user.IsValid });
            }

            return BadRequest("用户不存在");
        }

        public IActionResult OnPostToggleRole([FromBody] ToggleRequest data)
        {
            if (data == null || data.UserId == null)
            {
                return BadRequest("无效的请求数据");
            }

            int userId = data.UserId.Value;
            var user = DatabaseOperate.GetUserById(userId);
            if (user != null)
            {
                if(user.Role == UserMetadata.Role.User)
                {
                    user.Role = UserMetadata.Role.Admin;
                }
                else
                {
                    user.Role = UserMetadata.Role.User;
                }
                DatabaseOperate.UpdateUser(user);

                return new JsonResult(new { role = user.Role });
            }

            return BadRequest("用户不存在");
        }

        public class ToggleStateRequest
        {
            public int? UserId { get; set; }
        }

        public class ToggleRequest
        {
            public int? UserId { get; set; }
        }
    }
}
