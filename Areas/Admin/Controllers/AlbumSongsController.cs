using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotMusic.DataModel;
using System.Data.Entity;
using AutoMapper;
using HotMusic.Models;

namespace HotMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AlbumSongsController : Controller
    {
        private readonly MusicDbContext _context;

        public AlbumSongsController(MusicDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AlbumSongs
        public async Task<IActionResult> Index()
        {
            var listAlbumSong = from als in _context.AlbumSongs
                                join al in _context.Albums on als.AlbumId equals al.AlbumId
                                join s in _context.Songs on als.SongId equals s.SongId
                                select new AlbumSongs()
                                {
                                    SongId = als.SongId,
                                    AlbumId = als.AlbumId,
                                    SongTitle = s.SongTitle,
                                    AlbumTitle = al.AlbumTitle,
                                    CreatedBy = als.CreatedBy,
                                    CreatedDate = als.CreatedDate,
                                    ModifiedDate = als.ModifiedDate,
                                    ModifiledBy = als.ModifiledBy,
                                    IsDeleted = als.IsDeleted
                                };

            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<AlbumSongs, AlbumSongDisplayViewModel>();
            }).CreateMapper();
            var displayAlbumSong = mapper.Map<IEnumerable<AlbumSongDisplayViewModel>>(listAlbumSong);

            return _context.AlbumSongs != null ?
                        View(displayAlbumSong.ToList()) :
                        Problem("Entity set 'MusicDbContext.AlbumSongs'  is null.");
        }

        // GET: Admin/AlbumSongs/Details/5
        public async Task<IActionResult> Details(int? idSong, int? idAlbum)
        {
            if (idSong == null||idAlbum==null|| _context.AlbumSongs == null)
            {
                return NotFound();
            }

            var albumSongs = (from als in _context.AlbumSongs
                             join al in _context.Albums on als.AlbumId equals al.AlbumId
                             join s in _context.Songs on als.SongId equals s.SongId
                             select new AlbumSongs()
                             {
                                 SongId = als.SongId,
                                 AlbumId = als.AlbumId,
                                 SongTitle = s.SongTitle,
                                 AlbumTitle = al.AlbumTitle,
                                 CreatedBy = als.CreatedBy,
                                 CreatedDate = als.CreatedDate,
                                 ModifiedDate = als.ModifiedDate,
                                 ModifiledBy = als.ModifiledBy,
                                 IsDeleted = als.IsDeleted
                             }).First(als => als.SongId == idSong && als.AlbumId == idAlbum);
            if (albumSongs == null)
            {
                return NotFound();
            }
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<AlbumSongs, AlbumSongDisplayViewModel>();
            }).CreateMapper();
            var displayAlbumSong = mapper.Map<AlbumSongDisplayViewModel>(albumSongs);
            return View(displayAlbumSong);
        }

        // GET: Admin/AlbumSongs/Create
        public IActionResult Create()
        {
            LoadDrop();
            return View();
        }
        public void LoadDrop()
        {
            var listSong = from s in _context.Songs
                           select new SelectListItem()
                           {
                               Text = s.SongTitle,
                               Value = s.SongId.ToString()
                           };
            var listAlbum = from al in _context.Albums
                            select new SelectListItem()
                            {
                                Text = al.AlbumTitle,
                                Value = al.AlbumId.ToString()
                            };
            ViewBag.listSong = listSong.ToList();
            ViewBag.listAlbum = listAlbum.ToList();
        }
        // POST: Admin/AlbumSongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlbumId,SongId")] AlbumSongDisplayViewModel albumSongs)
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (ModelState.IsValid)
            {
                var mapper = new MapperConfiguration(config =>
                {
                    config.CreateMap<AlbumSongDisplayViewModel, AlbumSongs>()
                    .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
                    .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => userName));
                }).CreateMapper();
                var newAlbumSongs = mapper.Map<AlbumSongs>(albumSongs);
                _context.Add(newAlbumSongs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            LoadDrop();
            return View(albumSongs);
        }

        // GET: Admin/AlbumSongs/Edit/5
        public async Task<IActionResult> Edit(int? idSong, int? idAlbum)
        {
            LoadDrop();
            if (idAlbum == null ||idSong==null || _context.AlbumSongs == null)
            {
                return NotFound();
            }

            var albumSongs = (from als in _context.AlbumSongs
                              join al in _context.Albums on als.AlbumId equals al.AlbumId
                              join s in _context.Songs on als.SongId equals s.SongId
                              select new AlbumSongs()
                              {
                                  SongId = als.SongId,
                                  AlbumId = als.AlbumId,
                                  SongTitle = s.SongTitle,
                                  AlbumTitle = al.AlbumTitle,
                                  CreatedBy = als.CreatedBy,
                                  CreatedDate = als.CreatedDate,
                                  ModifiedDate = als.ModifiedDate,
                                  ModifiledBy = als.ModifiledBy,
                                  IsDeleted = als.IsDeleted
                              }).First(als => als.SongId == idSong && als.AlbumId == idAlbum);
            if (albumSongs == null)
            {
                return NotFound();
            }
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<AlbumSongs, AlbumSongDisplayViewModel>();
            }).CreateMapper();
            var displayAlbumSong = mapper.Map<AlbumSongDisplayViewModel>(albumSongs);
            return View(displayAlbumSong);
        }

        // POST: Admin/AlbumSongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? idSong, int? idAlbum, [Bind("AlbumId,SongId")] AlbumSongDisplayViewModel albumSongs)
        {
            if (idSong != albumSongs.SongId && idAlbum != albumSongs.AlbumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userName = HttpContext.Session.GetString("UserName");
                try
                {
                    var mapper = new MapperConfiguration(config =>
                    {
                        config.CreateMap<AlbumSongDisplayViewModel, AlbumSongs>()
                         .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.Now))
                        .ForMember(dest => dest.ModifiledBy, opt => opt.MapFrom(src => userName));
                    }).CreateMapper();
                    var newAlbumSongs = mapper.Map<AlbumSongs>(albumSongs);
                    _context.Update(newAlbumSongs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumSongsExists(albumSongs.SongId))
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
            LoadDrop();
            return View(albumSongs);
        }

        // GET: Admin/AlbumSongs/Delete/5
        public async Task<IActionResult> Delete(int? idSong, int? idAlbum)
        {
            if (idSong == null || idAlbum == null || _context.AlbumSongs == null)
            {
                return NotFound();
            }

            var albumSongs = (from als in _context.AlbumSongs
                             join al in _context.Albums on als.AlbumId equals al.AlbumId
                             join s in _context.Songs on als.SongId equals s.SongId
                             select new AlbumSongs()
                             {
                                 SongId = als.SongId,
                                 AlbumId = als.AlbumId,
                                 SongTitle = s.SongTitle,
                                 AlbumTitle = al.AlbumTitle,
                                 CreatedBy = als.CreatedBy,
                                 CreatedDate = als.CreatedDate,
                                 ModifiedDate = als.ModifiedDate,
                                 ModifiledBy = als.ModifiledBy,
                                 IsDeleted = als.IsDeleted
                             }).First(als => als.SongId == idSong && als.AlbumId == idAlbum);
            if (albumSongs == null)
            {
                return NotFound();
            }
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<AlbumSongs, AlbumSongDisplayViewModel>();
            }).CreateMapper();
            var displayAlbumSong = mapper.Map<AlbumSongDisplayViewModel>(albumSongs);
            return View(displayAlbumSong);
        }

        // POST: Admin/AlbumSongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int idSong, int? idAlbum)
        {
            if (_context.AlbumSongs == null)
            {
                return Problem("Entity set 'MusicDbContext.AlbumSongs'  is null.");
            }
            var albumSongs = _context.AlbumSongs.First(als => als.SongId == idSong && als.AlbumId == idAlbum);
            if (albumSongs != null)
            {
                _context.AlbumSongs.Remove(albumSongs);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumSongsExists(int id)
        {
            return (_context.AlbumSongs?.Any(e => e.SongId == id)).GetValueOrDefault();
        }
    }
}
