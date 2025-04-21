using Microsoft.AspNetCore.Mvc;

namespace LostAndFoundWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
