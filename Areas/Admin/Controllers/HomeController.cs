using HotMusic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotMusic.Models;
using System.Diagnostics;

namespace HotMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMusicService _musicService;

        public HomeController(ILogger<HomeController> logger, IMusicService musicService)
        {
            _logger = logger;
            _musicService = musicService;
        }

        public IActionResult Index()
        {
            // Get data from service

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}