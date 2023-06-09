using HotMusic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhac.Common;
using QuanLyNhac.DataModel;

namespace QuanLyNhac.Controllers
{
    [Authorize]
    public class PlaylistSongsController : Controller
    {
        private readonly MusicDbContext _context;
        private readonly int _pageSize = 5;

        public PlaylistSongsController(MusicDbContext context)
        {
            _context = context;
        }

        // GET: PlaylistSongs
        public async Task<IActionResult> Index(string filter, int? pageNumber)
        {
            if (string.IsNullOrEmpty(filter))
            {
                filter = string.Empty;
            }
            var listData = from pl in _context.PlaylistSongs
                           join song in _context.Songs on pl.SongId equals song.SongId
                           join playList in _context.Playlists on pl.PlaylistId equals playList.PlaylistId
                           select new PlaylistSongViewModel()
                           {
                               PlaylistId = pl.PlaylistId,
                               PlaylistName = playList.PlaylistTitle,
                               SongId = pl.SongId,
                               SongName = song.SongTitle
                           };
            var listResult = await listData.Where(s => s.PlaylistName.Contains(filter)).ToListAsync();

            return _context.PlaylistSongs != null ?
                        View(await PaginatedList<PlaylistSongViewModel>.CreateAsync(listResult, pageNumber ?? 1, _pageSize)) :
                        Problem("Entity set 'MusicDbContext.PlaylistSongs'  is null.");
        }

        // GET: PlaylistSongs/Details/5
        public async Task<IActionResult> Details(int? pId, int? sId)
        {
            if (pId == null || sId == null || _context.PlaylistSongs == null)
            {
                return NotFound();
            }

            var playlistSongs = from pl in _context.PlaylistSongs
                                join p in _context.Playlists on pl.PlaylistId equals p.PlaylistId
                                join s in _context.Songs on pl.SongId equals s.SongId
                                where pl.SongId == sId && pl.SongId == sId
                                select new PlaylistSongViewModel()
                                {
                                    SongId = pl.SongId,
                                    PlaylistId = pl.PlaylistId,
                                    PlaylistName = p.PlaylistTitle,
                                    SongName = s.SongTitle
                                };

            if (!playlistSongs.Any())
            {
                return NotFound();
            }

            var displayData = await playlistSongs.FirstOrDefaultAsync();
            return View(displayData);
        }

        // GET: PlaylistSongs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PlaylistSongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SongId,PlaylistId")] PlaylistSongViewModel playlistSongs)
        {
            if (ModelState.IsValid)
            {
                var mapperConfig = new AutoMapper.MapperConfiguration(config =>
                {
                    config.CreateMap<PlaylistSongViewModel, PlaylistSongs>();
                });
                var mapper = mapperConfig.CreateMapper();
                var newPlaylistSong = mapper.Map<PlaylistSongs>(playlistSongs);
                _context.Add(newPlaylistSong);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(playlistSongs);
        }

        // GET: PlaylistSongs/Edit/5
        public async Task<IActionResult> Edit(int? pId, int? sId)
        {
            if (pId == null || sId == null || _context.PlaylistSongs == null)
            {
                return NotFound();
            }

            var playlistSongs = from pl in _context.PlaylistSongs
                                join p in _context.Playlists on pl.PlaylistId equals p.PlaylistId
                                join s in _context.Songs on pl.SongId equals s.SongId
                                where pl.SongId == sId && pl.PlaylistId == pId
                                select new PlaylistSongViewModel()
                                {
                                    PlaylistId = pl.PlaylistId,
                                    PlaylistName = p.PlaylistTitle,
                                    SongId = pl.SongId,
                                    SongName = s.SongTitle
                                };
            if (!playlistSongs.Any())
            {
                return NotFound();
            }

            var displayObject = await playlistSongs.FirstOrDefaultAsync();
            return View(displayObject);
        }

        // POST: PlaylistSongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SongId,PlaylistId")] PlaylistSongViewModel playlistSongs)
        {
            if (id != playlistSongs.SongId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var mapperConfig = new AutoMapper.MapperConfiguration(config =>
                    {
                        config.CreateMap<PlaylistSongViewModel, PlaylistSongs>();
                    });
                    var mapper = mapperConfig.CreateMapper();
                    var updateData = mapper.Map<PlaylistSongs>(playlistSongs);

                    _context.Update(updateData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaylistSongsExists(playlistSongs.SongId))
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
            return View(playlistSongs);
        }

        // GET: PlaylistSongs/Delete/5
        public async Task<IActionResult> Delete(int? pId, int? sId)
        {
            if (pId == null || sId == null || _context.PlaylistSongs == null)
            {
                return NotFound();
            }

            var playlistSongs = await _context.PlaylistSongs
                .FirstOrDefaultAsync(m => m.SongId == sId && m.PlaylistId == pId);
            if (playlistSongs == null)
            {
                return NotFound();
            }

            return View(playlistSongs);
        }

        // POST: PlaylistSongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int PlaylistId, int SongId)
        {
            if (_context.PlaylistSongs == null)
            {
                return Problem("Entity set 'MusicDbContext.PlaylistSongs'  is null.");
            }
            var playlistSongs = await _context.PlaylistSongs.FindAsync(SongId, PlaylistId);
            if (playlistSongs != null)
            {
                _context.PlaylistSongs.Remove(playlistSongs);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlaylistSongsExists(int id)
        {
            return (_context.PlaylistSongs?.Any(e => e.SongId == id)).GetValueOrDefault();
        }
    }
}
