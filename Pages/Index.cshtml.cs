using LostAndFoundWebApp.Services.Mysql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGetItems()
        {
            // 从数据库获取所有物品数据
            var items = DatabaseOperate.GetAllItems(); // 假设已实现此方法
            return new JsonResult(items);
        }
    }
}
