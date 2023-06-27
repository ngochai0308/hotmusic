using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotMusic.DataModel;

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
              return _context.AlbumSongs != null ? 
                          View(await _context.AlbumSongs.ToListAsync()) :
                          Problem("Entity set 'MusicDbContext.AlbumSongs'  is null.");
        }

        // GET: Admin/AlbumSongs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AlbumSongs == null)
            {
                return NotFound();
            }

            var albumSongs = await _context.AlbumSongs
                .FirstOrDefaultAsync(m => m.SongId == id);
            if (albumSongs == null)
            {
                return NotFound();
            }

            return View(albumSongs);
        }

        // GET: Admin/AlbumSongs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AlbumSongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlbumId,SongId,CreatedDate,CreatedBy,ModifiedDate,ModifiledBy,IsDeleted")] AlbumSongs albumSongs)
        {
            if (ModelState.IsValid)
            {
                _context.Add(albumSongs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(albumSongs);
        }

        // GET: Admin/AlbumSongs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AlbumSongs == null)
            {
                return NotFound();
            }

            var albumSongs = await _context.AlbumSongs.FindAsync(id);
            if (albumSongs == null)
            {
                return NotFound();
            }
            return View(albumSongs);
        }

        // POST: Admin/AlbumSongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlbumId,SongId,CreatedDate,CreatedBy,ModifiedDate,ModifiledBy,IsDeleted")] AlbumSongs albumSongs)
        {
            if (id != albumSongs.SongId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(albumSongs);
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
            return View(albumSongs);
        }

        // GET: Admin/AlbumSongs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.AlbumSongs == null)
            {
                return NotFound();
            }

            var albumSongs = await _context.AlbumSongs
                .FirstOrDefaultAsync(m => m.SongId == id);
            if (albumSongs == null)
            {
                return NotFound();
            }

            return View(albumSongs);
        }

        // POST: Admin/AlbumSongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.AlbumSongs == null)
            {
                return Problem("Entity set 'MusicDbContext.AlbumSongs'  is null.");
            }
            var albumSongs = await _context.AlbumSongs.FindAsync(id);
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
