using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using LostAndFoundWebApp.Models;
using LostAndFoundWebApp.Services.Mysql;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace LostAndFoundWebApp.Pages
{
    public class ClaimApplicationModel(IWebHostEnvironment environment) : PageModel
    {

        private readonly IWebHostEnvironment _environment = environment;

        [BindProperty]
        public LostAndFoundWebApp.Models.Claim? Claim { get; set; }

        public void OnGet()
        {
            // 初始化默认值
            Claim = new LostAndFoundWebApp.Models.Claim
            {
                CreateTime = DateTime.Now,
                Status = ClaimMetadata.Status.DefaultStatus
            };
        }


        public IActionResult OnPost(IFormFile proofFile)
        {
            // 手动验证文件字段（绕过自动验证）
            if (proofFile == null || proofFile.Length == 0)
            {
                ModelState.AddModelError("proofFile", "必须上传证明文件");
                return Page();
            }

            if (Claim == null)
            {
                ModelState.AddModelError("Claim", "认领申请初始化失败");
                return Page();
            }

            int userID;
            try
            {
                var userIDClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                userID = int.Parse(userIDClaim ?? string.Empty);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "无法获取当前用户信息，请重新登录。";
                return RedirectToPage("/Index");
            }

            // 提前处理文件上传


            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "claims");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var guid = Guid.NewGuid();
            var proofFileIdx = 0;


            string fileName = $"{guid}-{proofFileIdx}{Path.GetExtension(proofFile.FileName)}";

            string filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                proofFile.CopyTo(stream);
            }

            Claim.ProofDocPath = $"/uploads/claims/{fileName}";

            // 设置用户ID（确保已授权）
            Claim.UserId = userID;

            // 手动验证其他字段
            if (Claim.ItemId <= 0)
            {
                ModelState.AddModelError("Claim.ItemId", "物品ID无效");
                return Page();
            }

            if (DatabaseOperate.GetItem(Claim.ItemId) == null)
            {
                ModelState.AddModelError("Claim.ItemId", "物品ID不存在");
                return Page();
            }


            // 数据库操作
            try
            {
                var result = DatabaseOperate.CreateClaim(Claim);
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"提交失败：{ex.Message}");
                return Page();
            }
        }
    }
}
