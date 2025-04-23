using LostAndFoundWebApp.Models;
using LostAndFoundWebApp.Services.Mysql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebApp.Pages
{
    [IgnoreAntiforgeryToken]
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
            var items = DatabaseOperate.GetAllItems();
            return new JsonResult(items);
        }

        [HttpPost]
        public IActionResult OnPostSearch([FromBody] ItemSearchParams SearchParams)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage);
                _logger.LogWarning("模型验证失败: {Errors}", string.Join(", ", errors));
                return BadRequest(new { errors });
            }

            try
            {
                _logger.LogInformation("收到搜索请求");
                var items = DatabaseOperate.SearchItems(SearchParams);
                return new JsonResult(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "搜索异常");
                return StatusCode(500, new { error = "服务器内部错误" });
            }
        }
    }
}
