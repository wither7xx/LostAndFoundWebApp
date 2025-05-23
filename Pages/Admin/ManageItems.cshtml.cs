using LostAndFoundWebApp.Models;
using LostAndFoundWebApp.Services.Mysql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebApp.Pages.Admin
{
    public class ManageItemsModel : PageModel
    {
        public List<Item> Items { get; set; } = [];

        public void OnGet()
        {
            Items = DatabaseOperate.GetAllItems();
        }

        public IActionResult OnGetGetImages(int itemId)
        {
            var images = DatabaseOperate.GetImagesByItem(itemId);
            if (images == null || images.Count == 0)
            {
                return new JsonResult(new List<object>()); // 返回空列表
            }

            return new JsonResult(images.Select(image => new
            {
                image.ImagePath
            }));
        }

        public IActionResult OnPostToggle([FromBody] ToggleRequest data)
        {
            if (data == null || data.ItemId == null)
            {
                return BadRequest("无效的请求数据");
            }

            int itemId = data.ItemId.Value;
            if (!DatabaseOperate.UpdateItem(itemId, false))
            {
                return BadRequest("更新物品有效性失败");
            }
            var item = DatabaseOperate.GetItem(itemId);
            if (item == null)
            {
                return BadRequest("物品不存在");
            }
            return new JsonResult(new
            {
                isValid = item.IsValid,
                isLostItem = item.Status == ItemMetadata.Status.Lost
            });
        }

        public class ToggleRequest
        {
            public int? ItemId { get; set; }
        }
    }
}
