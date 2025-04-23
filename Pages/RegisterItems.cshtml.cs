using LostAndFoundWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LostAndFoundWebApp.Pages
{
    public class RegisterItemsModel : PageModel
    {
        [BindProperty]
        public Item Item { get; set; } = new Item();
        public void OnGet()
        {
        }
    }
}
