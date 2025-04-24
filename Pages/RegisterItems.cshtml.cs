using LostAndFoundWebApp.Models;
using LostAndFoundWebApp.Services.Mysql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Security.Claims;

namespace LostAndFoundWebApp.Pages
{
    public class RegisterItemsModel(IWebHostEnvironment environment) : PageModel
    {
        private readonly IWebHostEnvironment _environment = environment;

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public string Status { get; set; } = string.Empty;

        [BindProperty]
        public string Location { get; set; } = string.Empty;

        [BindProperty]
        public string Campus { get; set; } = string.Empty;

        [BindProperty]
        public DateTime Time { get; set; } = DateTime.Now;

        [BindProperty]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public string ContactInfo { get; set; } = string.Empty;

        [BindProperty]
        public string Category { get; set; } = string.Empty;

        [BindProperty]
        public List<IFormFile>? Images { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid || Images == null)
            {
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

            // 保存物品信息到数据库
            int itemId = DatabaseOperate.CreateItem(new()
            {
                Name = Name,
                Status = Status,
                Location = Location,
                Campus = Campus,
                Time = Time,
                Description = Description,
                ContactInfo = ContactInfo,
                Category = Category,
                UserId = userID
            });

            // 保存图片到本地
            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "images");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var guid = Guid.NewGuid();
            var imageIdx = 0;
            foreach (var image in Images)
            {
                string fileName = $"{guid}-{imageIdx}{Path.GetExtension(image.FileName)}";
                imageIdx++;
                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                // 保存图片信息到数据库
                DatabaseOperate.CreateImage(new()
                {
                    ImagePath = $"/uploads/images/{fileName}",
                    ItemId = itemId
                });
            }

            return RedirectToPage("/Index");
        }
    }
}
