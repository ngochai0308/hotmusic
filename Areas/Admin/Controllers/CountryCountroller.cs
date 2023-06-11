using Microsoft.AspNetCore.Mvc;

namespace HotMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CountryCountroller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
