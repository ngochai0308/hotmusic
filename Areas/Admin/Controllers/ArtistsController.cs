using HotMusic.Contract;
using HotMusic.Models;
using HotMusic.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotMusic.Common;
using HotMusic.DataModel;
using AutoMapper;

namespace HotMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ArtistsController : Controller
    {
        private readonly IArtistRepository _artistRepository;
        private readonly int _pageSize = 5;

        public ArtistsController(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        // GET: Artists
        public async Task<IActionResult> Index(string filter, string sortOrder, int? pageNumber)
        {
            if (string.IsNullOrEmpty(filter))
            {
                filter = string.Empty;
            }
            ViewData["filter"] = filter;

            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "sortName";
            }
            ViewData["currentSort"] = sortOrder;
            ViewData["sortName"] = sortOrder == "NameASC" ? "NameDESC" : "NameASC";

            // Cach cũ: sử dụng thẳng context
            //var query = _context.Artists.Where(a => a.ArtistName.Contains(filter));

            // Cách mới
            var query = _artistRepository.GetAll(filter);


            List<Artists> listData = new List<Artists>();

            switch (sortOrder)
            {
                case "NameASC":
                    listData = query.OrderBy(a => a.ArtistName).ToList();
                    break;
                case "NameDESC":
                    listData = query.OrderByDescending(a => a.ArtistName).ToList();
                    break;
                default:
                    listData = query.OrderBy(a => a.ArtistName).ToList();
                    break;
            }

            var mapperConfig = new AutoMapper.MapperConfiguration(config =>
            {
                config.CreateMap<Artists, DisplayArtistViewModel>();
            });
            var mapper = mapperConfig.CreateMapper();
            var listResult = mapper.Map<List<DisplayArtistViewModel>>(listData);

            return _artistRepository != null ?
                        View(await PaginatedList<DisplayArtistViewModel>.CreateAsync(listResult, pageNumber ?? 1, _pageSize)) :
                        Problem("Entity set 'MusicDbContext.Artists'  is null.");
        }

        // GET: Artists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _artistRepository == null)
            {
                return NotFound();
            }

            var artist = _artistRepository.GetById(id.Value);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // GET: Artists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArtistId,ArtistName,Avatar,ArtistBio,FileUpload")] DisplayArtistViewModel artist)
        {
            if (ModelState.IsValid)
            {
                var file1 = string.Empty;
                if (artist.FileUpload != null)
                {
                    file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                       + Path.GetExtension(artist.FileUpload.FileName);
                    var file = Path.Combine("wwwroot", "img", "Artist", file1);
                    using var filestream = new FileStream(file, FileMode.Create);
                    await artist.FileUpload.CopyToAsync(filestream);
                }

                var mapper = new MapperConfiguration(config => {
                    config.CreateMap<DisplayArtistViewModel, Artists>();
                }).CreateMapper();

                var newArtist = mapper.Map<Artists>(artist);
                newArtist.Avatar = file1;

                _artistRepository.Add(newArtist);
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        // GET: Artists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _artistRepository == null)
            {
                return NotFound();
            }

            var artist = _artistRepository.GetById(id.Value);
            if (artist == null)
            {
                return NotFound();
            }
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Artists, DisplayArtistViewModel>();
            }).CreateMapper();
            var displaySong = mapper.Map<DisplayArtistViewModel>(artist);
            return View(displaySong);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArtistId,ArtistName,Avatar,ArtistBio,FileUpload")] DisplayArtistViewModel artist)
        {
            if (id != artist.ArtistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var checkArtist = _artistRepository.GetById(artist.ArtistId);
                    var oldFileName = checkArtist != null ? checkArtist.Avatar : string.Empty;
                    var file1 = oldFileName;

                    if (artist.FileUpload != null)
                    {
                        file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                           + Path.GetExtension(artist.FileUpload.FileName);
                        var file = Path.Combine("wwwroot", "img", "Artist", file1);
                        using var filestream = new FileStream(file, FileMode.Create);
                        await artist.FileUpload.CopyToAsync(filestream);
                    }

                    var mapper = new MapperConfiguration(config => {
                        config.CreateMap<DisplayArtistViewModel, Artists>();
                    }).CreateMapper();
                    if (!string.IsNullOrEmpty(oldFileName) && oldFileName != file1)
                    {
                        var fileOld = Path.Combine("wwwroot", "img", "Artist", oldFileName);
                        if (System.IO.File.Exists(fileOld))
                            System.IO.File.Delete(fileOld);
                    }
                    checkArtist = mapper.Map<Artists>(artist);
                    checkArtist.Avatar = file1;
                    _artistRepository.CheckTrackedArtist(artist.ArtistId);
                    _artistRepository.Update(checkArtist);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.ArtistId))
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
            return View(artist);
        }


        // GET: Artists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _artistRepository == null)
            {
                return NotFound();
            }

            var artist = _artistRepository.GetById(id.Value);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_artistRepository == null)
            {
                return Problem("Entity set 'MusicDbContext.Artists'  is null.");
            }
            _artistRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(int id)
        {
            return _artistRepository.GetById(id) != null;
        }
    }
}
