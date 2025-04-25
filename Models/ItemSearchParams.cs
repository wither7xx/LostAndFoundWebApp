namespace LostAndFoundWebApp.Models
{
    public class ItemSearchParams
    {
        public string? Name { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; } // 使用可空类型
        public DateTime? EndDate { get; set; }
        public string? Campus { get; set; }
        public bool? IsValid { get; set; }
        public bool? OnlyMyItems { get; set; }
        public int? UserID { get; set; }
        public string? Category { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
