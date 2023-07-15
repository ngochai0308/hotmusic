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
        private readonly IArtistRepository _artistRepository;
        private readonly MusicDbContext _context;

        public HomeController(ILogger<HomeController> logger, IMusicService musicService,IAlbumRepository albumRepository,ISongRepository songRepository,MusicDbContext context,IArtistRepository artistRepository)
        {
            _logger = logger;
            _songRepository = songRepository;
            _musicService = musicService;
            _albumRepository = albumRepository;
            _context= context;
            _artistRepository = artistRepository;
        }
        
        public IActionResult HomePage()
        {
            ViewData["listAlbum"] = _albumRepository.GetAll();
            ViewData["listAlbumHot"] = _albumRepository.GetAll().Where(a => a.ArtistId == 13 || a.ArtistId == 14).ToList();
            ViewData["Name"] = "Home";
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
            ViewData["Name"] = "Search";
            return View();
        }
        [Route("/search")]
        public IActionResult SearchResult(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return RedirectToAction("Search");
            }
            var listSong = _songRepository.GetAll(keyword);
            ViewData["listSong"] = listSong.ToList();

            ViewData["keyword"] = keyword;
            ViewData["NameId"] = "Songs";
            return View();
        }
        [Route("/search/{keyword?}/album")]
        public IActionResult AlbumResult(string keyword)
        {
            var listAlbum = _albumRepository.GetAll(keyword);

            ViewData["listAlbum"] = listAlbum.ToList();
            ViewData["keyword"] = keyword;
            ViewData["NameId"] = "Albums";
            return View();
        }
        [Route("/search/{keyword?}/playlist")]
        public IActionResult PlaylistResult(string keyword)
        {
            ViewData["keyword"] = keyword;
            ViewData["NameId"] = "Playlists";
            return View();
        }
        [Route("/search/{keyword?}/artist")]
        public IActionResult ArtistResult(string keyword)
        {
            var listArtist = _artistRepository.GetAll(keyword);
            ViewData["listArtist"] = listArtist.ToList();

            ViewData["keyword"] = keyword;
            ViewData["NameId"] = "Artists";
            return View();
        }
        public IActionResult DetailsArtist(int ArtistId)
        {
            var listAlbum = _albumRepository.GetAll().Where(al=>al.ArtistId == ArtistId);
            var Artist = _artistRepository.GetById(ArtistId);
            if(Artist == null)
            {
                NotFound();
            }
            ViewData["Artist"] = Artist;
            ViewData["listAlbum"] = listAlbum.ToList();
            return View();
        }
        [Route("/playSong/{id}")]
        public IActionResult PlaySong(int id)
        {
            var song = _songRepository.GetById(id);
            return PartialView("_PlaySongPartial", song);
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