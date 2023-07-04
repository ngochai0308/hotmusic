using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotMusic.DataModel;
using HotMusic.Models;
using AutoMapper;

namespace HotMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LyricsController : Controller
    {
        private readonly MusicDbContext _context;

        public LyricsController(MusicDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Lyrics
        public async Task<IActionResult> Index()
        {
            var listLyric = from l in _context.Lyric
                            join s in _context.Songs on l.SongId equals s.SongId
                            select new LyricDisplayViewModel()
                            {
                                Id = l.Id,
                                lyricc = l.lyricc,
                                SongId = l.SongId,
                                SongTitle = s.SongTitle
                            };
              return _context.Lyric != null ? 
                          View(await listLyric.ToListAsync()) :
                          Problem("Entity set 'MusicDbContext.Lyric'  is null.");
        }

        // GET: Admin/Lyrics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Lyric == null)
            {
                return NotFound();
            }

            var lyric = (from l in _context.Lyric
                        join s in _context.Songs on l.SongId equals s.SongId
                        select new LyricDisplayViewModel()
                        {
                            Id = l.Id,
                            lyricc = l.lyricc,
                            SongId = l.SongId,
                            SongTitle = s.SongTitle
                        }).FirstOrDefault(s=>s.Id==id); 
            if (lyric == null)
            {
                return NotFound();
            }
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Lyric, LyricDisplayViewModel>();
            }).CreateMapper();
            var displayLyric = mapper.Map<LyricDisplayViewModel>(lyric);
            return View(displayLyric);
        }

        // GET: Admin/Lyrics/Create
        public void LoadDropDownListSong()
        {
            var listSong = from s in _context.Songs
                           select new SelectListItem()
                           {
                               Text = s.SongTitle,
                               Value = s.SongId.ToString()
                           };
            ViewBag.ListSong = listSong.ToList();                  
        }
        public IActionResult Create()
        {
            LoadDropDownListSong();
            return View();
            
        }

        // POST: Admin/Lyrics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,lyricc,SongId")] LyricDisplayViewModel lyric)
        {
            var ab = lyric.lyricc;
            if (ModelState.IsValid)
            {
                var mapper = new MapperConfiguration(config =>
                {
                    config.CreateMap<LyricDisplayViewModel, Lyric>();
                }).CreateMapper();
                var newLyric = mapper.Map<Lyric>(lyric);
                _context.Add(newLyric);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            LoadDropDownListSong();
            return View(lyric);
        }

        // GET: Admin/Lyrics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Lyric == null)
            {
                return NotFound();
            }
            LoadDropDownListSong();
            var lyric = (from l in _context.Lyric
                        join s in _context.Songs on l.SongId equals s.SongId
                        select new LyricDisplayViewModel()
                        {
                            Id = l.Id,
                            lyricc = l.lyricc,
                            SongId = l.SongId,
                            SongTitle = s.SongTitle
                        }).FirstOrDefault(l => l.Id == id);
            if (lyric == null)
            {
                return NotFound();
            }
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Lyric, LyricDisplayViewModel>();
            }).CreateMapper();
            var displayLyric = mapper.Map<LyricDisplayViewModel>(lyric);
            return View(displayLyric);
        }

        // POST: Admin/Lyrics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,lyricc,SongId")] LyricDisplayViewModel lyric)
        {
            if (id != lyric.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var mapper = new MapperConfiguration(config =>
                    {
                        config.CreateMap<LyricDisplayViewModel, Lyric>();
                    }).CreateMapper();
                    var newLyric = mapper.Map<Lyric>(lyric);
                    _context.Update(newLyric);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LyricExists(lyric.Id))
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
            LoadDropDownListSong();
            return View(lyric);
        }

        // GET: Admin/Lyrics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Lyric == null)
            {
                return NotFound();
            }

            var lyric = (from l in _context.Lyric
                        join s in _context.Songs on l.SongId equals s.SongId
                        select new LyricDisplayViewModel()
                        {
                            Id = l.Id,
                            lyricc = l.lyricc,
                            SongId = l.SongId,
                            SongTitle = s.SongTitle
                        }).FirstOrDefault(l=>l.Id==id);
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Lyric, LyricDisplayViewModel>();
            }).CreateMapper();
            var displayLyric = mapper.Map<LyricDisplayViewModel>(lyric);
            if (lyric == null)
            {
                return NotFound();
            }

            return View(displayLyric);
        }

        // POST: Admin/Lyrics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Lyric == null)
            {
                return Problem("Entity set 'MusicDbContext.Lyric'  is null.");
            }
            var lyric = await _context.Lyric.FindAsync(id);
            if (lyric != null)
            {
                _context.Lyric.Remove(lyric);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LyricExists(int id)
        {
          return (_context.Lyric?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
