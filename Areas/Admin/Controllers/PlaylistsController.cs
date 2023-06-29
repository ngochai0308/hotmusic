using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using HotMusic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotMusic.Common;
using HotMusic.DataModel;

namespace HotMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PlaylistsController : Controller
    {
        private readonly MusicDbContext _context;
        private readonly int _pageSize = 5;

        public PlaylistsController(MusicDbContext context)
        {
            _context = context;
        }

        // GET: Playlists
        public async Task<IActionResult> Index(string filter, string sortOrder, int? pageNumber)
        {
            if (string.IsNullOrEmpty(filter))
            {
                filter = string.Empty;
            }
            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = string.Empty;
            }
            ViewData["filter"] = filter;
            ViewData["sortOrder"] = sortOrder;

            ViewData["orderByName"] = sortOrder == "NameASC" ? "NameDESC" : "NameASC";
            ViewData["orderByUser"] = sortOrder == "UserASC" ? "UserDESC" : "UserASC";

            var listData = _context.Playlists.Where(p => p.PlaylistTitle.Contains(filter));
            List<Playlists> listResult;
            switch (sortOrder)
            {
                case "NameDESC":
                    listResult = await listData.OrderByDescending(p => p.PlaylistTitle).ToListAsync();
                    break;
                case "UserASC":
                    listResult = await listData.OrderBy(p => p.UserId).ToListAsync();
                    break;
                case "UserDESC":
                    listResult = await listData.OrderByDescending(p => p.UserId).ToListAsync();
                    break;
                default:
                    listResult = await listData.OrderBy(p => p.PlaylistTitle).ToListAsync();
                    break;
            }

            // Using auto mapper
            var mapperConfig = new AutoMapper.MapperConfiguration(config =>
            {
                config.CreateMap<Playlists, PlaylistViewModel>();
            });
            var mapper = mapperConfig.CreateMapper();
            var listDisplay = mapper.Map<List<PlaylistViewModel>>(listData);

            return _context.Playlists != null ?
                        View(await PaginatedList<PlaylistViewModel>.CreateAsync(listDisplay, pageNumber ?? 1, _pageSize)) :
                        Problem("Entity set 'MusicDbContext.Playlists'  is null.");
        }

        // GET: Playlists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Playlists == null)
            {
                return NotFound();
            }

            var playlists = await _context.Playlists
                .FirstOrDefaultAsync(m => m.PlaylistId == id);
            if (playlists == null)
            {
                return NotFound();
            }

            return View(playlists);
        }

        // GET: Playlists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Playlists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlaylistId,PlaylistTitle,UserId")] Playlists playlists)
        {
            if (ModelState.IsValid)
            {
                _context.Add(playlists);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(playlists);
        }

        // GET: Playlists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Playlists == null)
            {
                return NotFound();
            }

            var playlists = await _context.Playlists.FindAsync(id);
            if (playlists == null)
            {
                return NotFound();
            }
            return View(playlists);
        }

        // POST: Playlists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlaylistId,PlaylistTitle,UserId")] Playlists playlists)
        {
            if (id != playlists.PlaylistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playlists);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaylistsExists(playlists.PlaylistId))
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
            return View(playlists);
        }

        // GET: Playlists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Playlists == null)
            {
                return NotFound();
            }

            var playlists = await _context.Playlists
                .FirstOrDefaultAsync(m => m.PlaylistId == id);
            if (playlists == null)
            {
                return NotFound();
            }

            return View(playlists);
        }

        // POST: Playlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Playlists == null)
            {
                return Problem("Entity set 'MusicDbContext.Playlists'  is null.");
            }
            var playlists = await _context.Playlists.FindAsync(id);
            if (playlists != null)
            {
                _context.Playlists.Remove(playlists);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlaylistsExists(int id)
        {
            return (_context.Playlists?.Any(e => e.PlaylistId == id)).GetValueOrDefault();
        }
    }
}
