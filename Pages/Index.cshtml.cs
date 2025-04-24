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

        public class PaginatedResult<T>
        {
            public List<T> Items { get; set; }
            public int TotalItems { get; set; }
            public int CurrentPage { get; set; }
            public int PageSize { get; set; }
            public int TotalPages { get; set; }

            public PaginatedResult(List<T> items, int totalItems, int currentPage, int pageSize)
            {
                Items = items;
                TotalItems = totalItems;
                CurrentPage = currentPage;
                PageSize = pageSize;
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            }
        }

        public IActionResult OnGetItems([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // 确保页码和每页数量在合理范围内
                page = Math.Max(1, page);
                pageSize = Math.Clamp(pageSize, 10, 50);

                // 获取总记录数
                int totalItems = DatabaseOperate.GetItemsCount();

                // 计算偏移量
                int skip = (page - 1) * pageSize;

                // 从数据库获取分页后的物品数据
                var items = DatabaseOperate.GetPaginatedItems(skip, pageSize);

                var result = new PaginatedResult<Item>(
                    items,
                    totalItems,
                    page,
                    pageSize
                );

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取物品列表时发生错误");
                return StatusCode(500, new { error = "获取数据失败" });
            }
        }

        public class SearchResult
        {
            public List<Item> Items { get; set; }
            public int TotalItems { get; set; }
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
        }

        [HttpPost]
        public JsonResult OnPostSearch([FromBody] ItemSearchParams searchParams)
        {
            try
            {
                var result = DatabaseOperate.SearchItems(searchParams);
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "搜索失败");
                return new JsonResult(new { error = "搜索失败" }) { StatusCode = 500 };
            }
        }
    }
}