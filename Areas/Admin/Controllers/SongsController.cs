using HotMusic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotMusic.Common;
using HotMusic.DataModel;
using System.Net.Http;

namespace HotMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SongsController : Controller
    {
        private readonly MusicDbContext _context;
        private readonly int _pageSize = 5;

        public SongsController(MusicDbContext context)
        {
            _context = context;
        }

        // GET: Songs CTR+M+O
        public async Task<IActionResult> Index(string filter, string sortOrder, int? pageNumber)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                filter = string.Empty;
            }

            HttpContext.Session.SetString("filter", filter);

            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "Name_ASC";
            }
            ViewData["CurrentSort"] = sortOrder;

            ViewData["filter"] = filter;
            ViewData["SortSongName"] = sortOrder == "Name_DESC" ? "Name_ASC" : "Name_DESC";
            ViewData["SortAName"] = sortOrder == "AName_ASC" ? "AName_DESC" : "AName_ASC";
            ViewData["SortAlbumName"] = sortOrder == "AlbumName_ASC" ? "AlbumName_DESC" : "AlbumName_ASC";
            ViewData["SortAlbumId"] = sortOrder == "AlbumId_ASC" ? "AlbumId_DESC" : "AlbumId_ASC";

            var listData = from s in _context.Songs
                           join a in _context.Artists on s.ArtistId equals a.ArtistId
                           join b in _context.Albums on s.AlbumId equals b.AlbumId
                           select new SongViewModel()
                           {
                               AlbumId = s.AlbumId,
                               AlbumName = b.AlbumTitle,
                               ArtistId = a.ArtistId,
                               ArtistName = a.ArtistName,
                               SongId = s.SongId,
                               SongTitle = s.SongTitle,
                               SongUrl = s.SongUrl,
                               ViewCount = s.ViewCount
                           };
            // Unit test = kiem thu don vi
            // Automation test => tester

            // sum(a,b) = sum(3,5) = 8 => 8? FAILed
            // Code coverage = 80% =, 60%

            var listResult = await listData.Where(song => song.SongTitle.Contains(filter)).ToListAsync();

            switch (sortOrder)
            {
                case "Name_DESC":
                    listResult = listResult.OrderByDescending(s => s.SongTitle).ToList();
                    break;
                case "AName_ASC":
                    listResult = listResult.OrderBy(s => s.ArtistName).ToList();
                    break;
                case "AName_DESC":
                    listResult = listResult.OrderByDescending(s => s.ArtistName).ToList();
                    break;
                case "AlbumName_ASC":
                    listResult = listResult.OrderBy(s => s.AlbumName).ToList();
                    break;
                case "AlbumName_DESC":
                    listResult = listResult.OrderByDescending(s => s.AlbumName).ToList();
                    break;
                case "AlbumId_ASC":
                    listResult = listResult.OrderBy(s => s.AlbumId).ToList();
                    break;
                case "AlbumId_DESC":
                    listResult = listResult.OrderByDescending(s => s.AlbumId).ToList();
                    break;
                default:
                    listResult = listResult.OrderBy(s => s.SongTitle).ToList();
                    break;
            }

            return _context.Songs != null ?
                        View(await PaginatedList<SongViewModel>.CreateAsync(listResult, pageNumber ?? 1, _pageSize)) :
                        Problem("Entity set 'MusicDbContext.Songs'  is null.");
        }

        // GET: Songs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var songs = await _context.Songs
                .FirstOrDefaultAsync(m => m.SongId == id);
            if (songs == null)
            {
                return NotFound();
            }

            // Su dung auto mapper
            var mapperConfig = new AutoMapper.MapperConfiguration(config =>
            {
                config.CreateMap<Songs, SongViewModel>();
            });

            var mapper = mapperConfig.CreateMapper();
            var displaySong = mapper.Map<SongViewModel>(songs);

            return View(displaySong);
        }

        private async Task LoadDropdownlistDataSync()
        {
            var listArtist = from a in _context.Artists
                             select new SelectListItem()
                             {
                                 Value = a.ArtistId.ToString(),
                                 Text = a.ArtistName
                             };

            var listAlbum = from a in _context.Albums
                            select new SelectListItem()
                            {
                                Value = a.AlbumId.ToString(),
                                Text = a.AlbumTitle
                            };
            // ViewBag: Dùng để mang dữ liệu từ Controller => View vẫn dùng được
            ViewBag.listArtist = await listArtist.ToListAsync();
            ViewBag.listAlbum = await listAlbum.ToListAsync();
        }

        // GET: Songs/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdownlistDataSync();

            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SongId,SongTitle,ArtistId,AlbumId,ViewCount,SongUrl")] SongViewModel songs)
        {
            if (ModelState.IsValid)
            {
                var mapperConfig = new AutoMapper.MapperConfiguration(config =>
                {
                    config.CreateMap<SongViewModel, Songs>();
                    //.ForMember(des=>des.SongTitle, source=> source.MapFrom(s=>s.AlbumName));
                    // Map 2 thuộc tính có tên khác nhau của 2 đối tượng khác nhau
                });
                var mapper = mapperConfig.CreateMapper();
                var newSong = mapper.Map<Songs>(songs);

                _context.Add(newSong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdownlistDataSync();

            return View(songs);
        }

        // GET: Songs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var songs = await _context.Songs.FindAsync(id);
            if (songs == null)
            {
                return NotFound();
            }

            await LoadDropdownlistDataSync();

            // Su dung auto mapper
            var mapperConfig = new AutoMapper.MapperConfiguration(config =>
            {
                config.CreateMap<Songs, SongViewModel>();
            });

            var mapper = mapperConfig.CreateMapper();
            var displaySong = mapper.Map<SongViewModel>(songs);

            return View(displaySong);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SongId,SongTitle,ArtistId,AlbumId,ViewCount,SongUrl")] SongViewModel songs)
        {
            if (id != songs.SongId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var mapperConfig = new AutoMapper.MapperConfiguration(config =>
                    {
                        config.CreateMap<SongViewModel, Songs>();
                    });
                    var mapper = mapperConfig.CreateMapper();
                    var updateSong = mapper.Map<Songs>(songs);
                    _context.Update(updateSong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongsExists(songs.SongId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdownlistDataSync();
            return View(songs);
        }

        // GET: Songs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Songs == null)
            {
                return NotFound();
            }

            var songs = await _context.Songs
                .FirstOrDefaultAsync(m => m.SongId == id);
            if (songs == null)
            {
                return NotFound();
            }

            return View(songs);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Songs == null)
            {
                return Problem("Entity set 'MusicDbContext.Songs'  is null.");
            }
            var songs = await _context.Songs.FindAsync(id);
            if (songs != null)
            {
                _context.Songs.Remove(songs);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongsExists(int id)
        {
            return (_context.Songs?.Any(e => e.SongId == id)).GetValueOrDefault();
        }
    }
}
