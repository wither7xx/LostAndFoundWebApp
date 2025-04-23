using LostAndFoundWebApp.Models;
using LostAndFoundWebApp.Services.Email;
using LostAndFoundWebApp.Services.Mysql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LostAndFoundWebApp.Pages.Admin
{
    public class ManageClaimsModel(IEmailSender emailSender) : PageModel
    {
        private readonly IEmailSender _emailSender = emailSender;
        public List<Models.Claim> Claims { get; set; } = [];

        public void OnGet()
        {
            Claims = DatabaseOperate.GetAllClaims();
        }

        public async Task<IActionResult> OnPostToggle([FromBody] ToggleRequest data)
        {
            if (data == null || data.ClaimId == null || data.NextStatus == null)
            {
                return BadRequest("无效的请求数据");
            }

            int claimId = data.ClaimId.Value;
            string nextStatus = data.NextStatus;
            if (DatabaseOperate.UpdateClaimStatus(claimId, nextStatus) <= 0)
            {
                return BadRequest("更新认领状态失败");
            }

            var claim = DatabaseOperate.GetClaim(claimId);
            if (claim == null)
            {
                return BadRequest("认领请求不存在");
            }
            var item = DatabaseOperate.GetItem(claim.ItemId);
            var user = DatabaseOperate.GetUserById(claim.UserId);
            if (user != null && item != null)
            {
                string subject = "认领申请通过";
                string message;
                if (data.NextStatus == ClaimMetadata.Status.Approved)
                {
                    message = $"您于{claim.CreateTime}对物品“{item.Name}”的认领申请已被批准，请尽快领取您的物品。";
                }
                else
                {
                    message = $"您于{claim.CreateTime}对物品“{item.Name}”的认领申请已被拒绝。";
                }
                await _emailSender.SendEmailAsync(user.Email, subject, message);
            }

            return new JsonResult(new { status = nextStatus });
        }

        public class ToggleRequest
        {
            public int? ClaimId { get; set; }
            public string? NextStatus { get; set; }
        }
    }
}
