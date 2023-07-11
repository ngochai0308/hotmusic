using HotMusic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotMusic.Models;
using System.Diagnostics;
using HotMusic.Contract;
using HotMusic.DataModel;

namespace HotMusic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMusicService _musicService;
        private readonly IAlbumRepository _albumRepository;
        private readonly ISongRepository _songRepository;
        private readonly MusicDbContext _context;

        public HomeController(ILogger<HomeController> logger, IMusicService musicService,IAlbumRepository albumRepository,ISongRepository songRepository,MusicDbContext context)
        {
            _logger = logger;
            _songRepository = songRepository;
            _musicService = musicService;
            _albumRepository = albumRepository;
            _context= context;
        }
        
        public IActionResult HomePage()
        {
            return View();
        }
        public IActionResult HomePage1()
        {
            ViewData["listAlbum"] = _albumRepository.GetAll();
            return View();
        }
        public IActionResult PageListSong(int albumId)
        {
            if (albumId == null || _albumRepository.GetById(albumId) == null)
            {
                return NotFound();
            }
            var ListIdSong = _context.AlbumSongs.Where(als => als.AlbumId == albumId)
                .Select(als => als.SongId);
            var listSong = _songRepository.GetAll().Where(s=> ListIdSong.Contains(s.SongId));
            var Album = _albumRepository.GetAll().Where(a => a.AlbumId == albumId).First();


            ViewData["listSongg"] = listSong;
            ViewData["Album"] = Album;
            return View();
        }
        public IActionResult Search()
        {
            var listAlbum = _albumRepository.GetAll();
            ViewData["listAlbum"] = listAlbum.ToList();
            return View();
        }
        public IActionResult SearchResult(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return RedirectToAction("Search");
            }
            ViewData["keyword"] = keyword;
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