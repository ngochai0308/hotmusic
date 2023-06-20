using HotMusic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotMusic.Common;
using HotMusic.DataModel;
using System.Net.Http;
using HotMusic.Contract;
using AutoMapper;
using HotMusic.Repository;
using System.Text;

namespace HotMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SongsController : Controller
    {
        private readonly ISongRepository _songRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly int _pageSize = 5;

        public SongsController(ISongRepository songRepository, IArtistRepository artistRepository, ICategoryRepository categoryRepository)
        {
            _songRepository = songRepository;
            _artistRepository = artistRepository;
            _categoryRepository = categoryRepository;
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

            var listResult = _songRepository.GetAll(filter);

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
                default:
                    listResult = listResult.OrderBy(s => s.SongTitle).ToList();
                    break;
            }
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Songs, SongViewModel>();
            }).CreateMapper();
            var listSongs = mapper.Map<IEnumerable<SongViewModel>>(listResult);

            return _songRepository != null ?
                        View(await PaginatedList<SongViewModel>.CreateAsync(listSongs, pageNumber ?? 1, _pageSize)) :
                        Problem("Entity set 'MusicDbContext.Songs'  is null.");
        }

        // GET: Songs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _songRepository == null)
            {
                return NotFound();
            }

            var songs = _songRepository.GetById(id.Value);
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

        private async Task LoadDropdownlistData()
        {
            var listArtist = from a in _artistRepository.GetAll()
                             select new SelectListItem()
                             {
                                 Value = a.ArtistId.ToString(),
                                 Text = a.ArtistName
                             };

            var listCategory = from a in  _categoryRepository.GetAll()
                            select new SelectListItem()
                            {
                                Value = a.CategoryId.ToString(),
                                Text = a.CategoryTitle
                            };
            // ViewBag: Dùng để mang dữ liệu từ Controller => View vẫn dùng được
            ViewBag.listArtist = listArtist.ToList();
            ViewBag.listCategory = listCategory.ToList();
        }

        // GET: Songs/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdownlistData();
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SongId,SongTitle,CategoryId,ArtistId,ViewCount,SongUrl,FileUpload")] SongViewModel songs)
        {
            if (ModelState.IsValid)
            {
                var userName = HttpContext.Session.GetString("UserName");
                var file1 = string.Empty;
                if (songs.FileUpload != null)
                {
                    file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                            + Path.GetExtension(songs.FileUpload.FileName);

                    var file = Path.Combine("wwwroot", "img", "Song", file1);

                    using var filestream = new FileStream(file, FileMode.Create);
                    await songs.FileUpload.CopyToAsync(filestream);
                }
                var mapperConfig = new AutoMapper.MapperConfiguration(config =>
                {
                    config.CreateMap<SongViewModel, Songs>()
                    .ForMember(dest => dest.Image, opt => opt.MapFrom(src => file1))
                    .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
                    .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => userName));
                });
                var mapper = mapperConfig.CreateMapper();
                var newSong = mapper.Map<Songs>(songs);

                _songRepository.Add(newSong);
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdownlistData();

            return View(songs);
        }

        // GET: Songs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _songRepository == null)
            {
                return NotFound();
            }

            var songs = _songRepository.GetById(id.Value);
            if (songs == null)
            {
                return NotFound();
            }

            await LoadDropdownlistData();

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
        public async Task<IActionResult> Edit(int id, [Bind("SongId,SongTitle,CategoryId,ArtistId,ViewCount,SongUrl,FileUpload")] SongViewModel songs)
        {
            if (id != songs.SongId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userName = HttpContext.Session.GetString("UserName");
                try
                {
                    var checkSong = _songRepository.GetById(songs.SongId);
                    var oldFileName = checkSong != null ? checkSong.Image : string.Empty;
                    var file1 = oldFileName;

                    if (songs.FileUpload != null)
                    {
                        file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                        + Path.GetExtension(songs.FileUpload.FileName);
                        var file = Path.Combine("wwwroot", "img", "Song", file1);
                        using var filestream = new FileStream(file, FileMode.Create);
                        await songs.FileUpload.CopyToAsync(filestream);
                    }
                    if (!string.IsNullOrEmpty(oldFileName) && oldFileName != file1)
                    {
                        var fileOld = Path.Combine("wwwroot", "img", "Song", oldFileName);
                        if (System.IO.File.Exists(fileOld))
                            System.IO.File.Delete(fileOld);
                    }

                    var mapperConfig = new AutoMapper.MapperConfiguration(config =>
                    {
                        config.CreateMap<SongViewModel, Songs>()
                        .ForMember(dest => dest.Image, opt => opt.MapFrom(src => file1))
                        .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.Now))
                        .ForMember(dest => dest.ModifiledBy, opt => opt.MapFrom(src => userName));
                    });
                    var mapper = mapperConfig.CreateMapper();

                    var updateSong = mapper.Map<Songs>(songs);

                    _songRepository.Update(updateSong);


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

            await LoadDropdownlistData();
            return View(songs);
        }

        // GET: Songs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _songRepository == null)
            {
                return NotFound();
            }
            

            var songs = _songRepository.GetById(id.Value);
            if (songs == null)
            {
                return NotFound();
            }
            var mapperConfig = new AutoMapper.MapperConfiguration(config =>
            {
                config.CreateMap<Songs, SongViewModel>();
            });

            var mapper = mapperConfig.CreateMapper();
            var displaySong = mapper.Map<SongViewModel>(songs);

            return View(displaySong);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_songRepository == null)
            {
                return Problem("Entity set 'MusicDbContext.Songs'  is null.");
            }
            //Delete image in wwwroot
            var checkSong = _songRepository.GetById(id);
            var oldFileName = checkSong != null ? checkSong.Image : string.Empty;

            if (!string.IsNullOrEmpty(oldFileName))
            {
                var fileOld = Path.Combine("wwwroot", "img", "Song", oldFileName);
                if (System.IO.File.Exists(fileOld))
                    System.IO.File.Delete(fileOld);
            }
            _songRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private bool SongsExists(int id)
        {
            return _songRepository.GetById(id) != null;
        }

        public FileResult ExportCSV()
        {
            string[] columnNames = new string[] { "SongId", "SongTitle", "Image", "ArtistId", "ArtistName", "CategoryID", "CategoryTitle", "CreatedDate", "CreatedBy", "ModifiedDate", "ModifiledBy" };

            var listSong = _songRepository.GetAll();
            string csv = string.Empty;
            foreach (var column in columnNames)
            {
                csv += column + ",";
            }
            csv += "\r\n";
            foreach (var song in listSong)
            {
                csv += song.SongId.ToString().Replace(",", ";") + ',';
                csv += song.SongTitle.Replace(",", ";") + ',';
                csv += song.Image?.Replace(",", ";") + ',';
                csv += song.ArtistId.ToString().Replace(",", ";") + ',';
                csv += song.ArtistName?.Replace(",", ";") + ',';
                csv += song.CategoryId.ToString().Replace(",", ";") + ',';
                csv += song.CategoryTitle?.Replace(",", ";") + ',';
                csv += song.CreatedDate?.ToString().Replace(",", ";") + ',';
                csv += song.CreatedBy?.Replace(",", ";") + ',';
                csv += song.ModifiedDate?.ToString().Replace(",", ";") + ',';
                csv += song.ModifiledBy?.Replace(",", ";") + ',';
                csv += "\r\n";
            }
            byte[] bytes = Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "song.csv");
        }
    }
}
