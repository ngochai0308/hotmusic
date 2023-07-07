using HotMusic.Contract;
using HotMusic.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotMusic.Common;
using HotMusic.DataModel;
using System.Text;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace HotMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AlbumsController : Controller
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly int _pageSize = 5; // Max 5 records/page

        public AlbumsController(IAlbumRepository albumRepository, IArtistRepository artistRepository, ICategoryRepository categoryRepository)
        {
            _albumRepository = albumRepository;
            _artistRepository = artistRepository;
            _categoryRepository = categoryRepository;
        }

        // GET: Albums
        public async Task<IActionResult> Index(string filter, string sortOrder, int? pageNumber)
        {
            if (string.IsNullOrEmpty(filter))
            {
                filter = string.Empty;
            }
            ViewData["currentFilter"] = filter;

            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "sortTitle";
            }
            ViewData["currentSort"] = sortOrder;

            ViewData["SortTitle"] = sortOrder == "TitleASC" ? "TitleDESC" : "TitleASC";
            ViewData["SortId"] = sortOrder == "IdASC" ? "IdDESC" : "IdASC";


            var listData = _albumRepository.GetAll(filter);

            List<Albums> listResult;
            // Order result by sort order
            switch (sortOrder)
            {
                case "TitleASC":
                    listResult = listData.OrderBy(a => a.AlbumTitle).ToList();
                    break;
                case "TitleDESC":
                    listResult = listData.OrderByDescending(a => a.AlbumTitle).ToList();
                    break;
                case "IdASC":
                    listResult = listData.OrderBy(a => a.ArtistId).ToList();
                    break;
                case "IdDESC":
                    listResult = listData.OrderByDescending(a => a.ArtistId).ToList();
                    break;
                default:
                    listResult = listData.OrderBy(a => a.AlbumTitle).ToList();
                    break;
            }

            // Su dung auto mapper
            var mapperConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Albums, AlbumViewModel>();
            });
            var mapper = mapperConfig.CreateMapper();
            var listDisplay = mapper.Map<List<AlbumViewModel>>(listData);

            return _albumRepository != null ?
                        View(await PaginatedList<AlbumViewModel>.CreateAsync(listDisplay, pageNumber ?? 1, _pageSize)) :
                        Problem("Entity set 'MusicDbContext.Albums'  is null.");
        }


        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _albumRepository == null)
            {
                return NotFound();
            }

            var albums = _albumRepository.GetById(id.Value);
            var mapper = new MapperConfiguration(configure =>
            {
                configure.CreateMap<Albums, AlbumViewModel>();
            }).CreateMapper();
            var displayAlbum = mapper.Map<AlbumViewModel>(albums);
            if (albums == null)
            {
                return NotFound();
            }

            return View(displayAlbum);
        }

        private async Task LoadDropdownAsync()
        {
            var listDataArtist = _artistRepository.GetAll();
            var listDataCategory = _categoryRepository.GetAll();

            var listArtist = new List<SelectListItem>();
            foreach (var item in listDataArtist)
            {
                listArtist.Add(new SelectListItem()
                {
                    Text = item.ArtistName,
                    Value = item.ArtistId.ToString()
                });
            }
            var listCategory = new List<SelectListItem>();
            foreach (var item in listDataCategory)
            {
                listCategory.Add(new SelectListItem()
                {
                    Text = item.CategoryTitle,
                    Value = item.CategoryId.ToString()
                });
            }

            ViewBag.listArtist = listArtist;
            ViewBag.listCategory = listCategory;
        }
        // GET: Albums/Create
        public async Task<IActionResult> Create()
        {
            // Get artist list for combobox
            await LoadDropdownAsync();

            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlbumId,AlbumTitle,ArtistId,CategoryID,Thumbnail,FileUpload")] AlbumViewModel albums)
        {

            if (ModelState.IsValid)
            {
                var userName = HttpContext.Session.GetString("UserName");

                var file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                    + Path.GetExtension(albums.FileUpload.FileName);

                var file = Path.Combine("wwwroot", "img", "Album", file1);

                using var filestream = new FileStream(file, FileMode.Create);
                await albums.FileUpload.CopyToAsync(filestream);


                // Mapping to DB model
                var mapperConfig = new MapperConfiguration(config =>
                {
                    config.CreateMap<AlbumViewModel, Albums>()
                    .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom(src => file1))
                    .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
                    .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => userName));
                });

                var mapper = mapperConfig.CreateMapper();
                var newAlbum = mapper.Map<Albums>(albums);

                _albumRepository.Add(newAlbum);

                return RedirectToAction(nameof(Index));
            }
            await LoadDropdownAsync();
            return View(albums);
        }

        // GET: Albums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _albumRepository == null)
            {
                return NotFound();
            }

            var albums = _albumRepository.GetById(id.Value);

            if (albums == null)
            {
                return NotFound();
            }
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Albums, AlbumViewModel>();
            }).CreateMapper();
            var displayAlbum = mapper.Map<AlbumViewModel>(albums);
            await LoadDropdownAsync();

            return View(displayAlbum);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlbumId,AlbumTitle,ArtistId,CategoryID,Thumbnail,FileUpload")] AlbumViewModel albums)
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (id != albums.AlbumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var checkAlbum = _albumRepository.GetById(albums.AlbumId);
                    var oldFileName = checkAlbum != null ? checkAlbum.Thumbnail : string.Empty;
                    var file1 = oldFileName;

                    if (albums.FileUpload != null)
                    {
                        file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                        + Path.GetExtension(albums.FileUpload.FileName);
                        var file = Path.Combine("wwwroot", "img", "Album", file1);
                        using var filestream = new FileStream(file, FileMode.Create);
                        await albums.FileUpload.CopyToAsync(filestream);
                    }
                    if (!string.IsNullOrEmpty(oldFileName) && oldFileName != file1)
                    {
                        var fileOld = Path.Combine("wwwroot", "img", "Album", oldFileName);
                        if (System.IO.File.Exists(fileOld))
                            System.IO.File.Delete(fileOld);
                    }
                    var mapperConfig = new MapperConfiguration(config =>
                    {
                        config.CreateMap<AlbumViewModel, Albums>()
                        .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom(src => file1))
                        .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.Now))
                        .ForMember(dest => dest.ModifiledBy, opt => opt.MapFrom(src => userName));
                    });
                    var mapper = mapperConfig.CreateMapper();

                    var updateAlbum = mapper.Map<Albums>(albums);

                    _albumRepository.Update(updateAlbum);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumsExists(albums.AlbumId))
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
            // Get data for dropdownlist
            await LoadDropdownAsync();
            return View(albums);
        }

        // GET: Albums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _albumRepository == null)
            {
                return NotFound();
            }

            var albums = _albumRepository.GetById(id.Value);
            if (albums == null)
            {
                return NotFound();
            }
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Albums, AlbumViewModel>();
            }).CreateMapper();
            var displayAlbum = mapper.Map<AlbumViewModel>(albums);

            return View(displayAlbum);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_albumRepository == null)
            {
                return Problem("Entity set 'MusicDbContext.Albums'  is null.");
            }
            var checkAlbum = _albumRepository.GetById(id);
            var oldFileName = checkAlbum != null ? checkAlbum.Thumbnail : string.Empty;

            if (!string.IsNullOrEmpty(oldFileName))
            {
                var fileOld = Path.Combine("wwwroot", "img", "Album", oldFileName);
                if (System.IO.File.Exists(fileOld))
                    System.IO.File.Delete(fileOld);
            }
            _albumRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
        public FileResult ExportCSV()
        {
            string[] columnNames = new string[] { "Mã Album", "Tiêu đề", "Ảnh Album", "Mã nghệ sĩ", "Tên nghệ sĩ", "Mã thể loại", "Tên thể loại", "Ngày tạo", "Người tạo", "Ngày thay đổi", "Người thay đổi" };

            var listAlbum = _albumRepository.GetAll();
            string csv = string.Empty;
            foreach (var column in columnNames)
            {
                csv += column + ",";
            }
            csv += "\r\n";
            foreach (var album in listAlbum)
            {
                csv += album.AlbumId+",";
                csv += album.AlbumTitle + ",";
                csv += album.Thumbnail + ",";
                csv += album.ArtistId + ",";
                csv += album.ArtistName + ",";
                csv += album.CategoryID + ",";
                csv += album.CategoryTitle + ",";
                csv += album.CreatedDate + ",";
                csv += album.CreatedBy + ",";
                csv += album.ModifiedDate + ",";                
                csv += album.ModifiledBy;
                csv += "\r\n";
            }
            byte[] bytes = Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "album.csv");
        }
        [Route("/ImportCSVForAlbum")]
        public IActionResult GetFormImportCSV()
        {
            return PartialView("_ImportCSVAlbumPartialView"); 
        }
        [HttpPost]
        public IActionResult ImportCSV(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            string filePath = Path.Combine("wwwroot", "csv", csvFile.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                csvFile.CopyTo(stream);
            }
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HeaderValidated = null,
                MissingFieldFound=null
            };
            var listAlbum = _albumRepository.GetAll();
            var reader = new StreamReader(filePath);
            using (var csv = new CsvReader(reader, configuration))
            {
                csv.Context.RegisterClassMap<AlbumMap>();
                var records = csv.GetRecords<Albums>();
                foreach(var record in records)
                {
                    bool isExists = false;
                    foreach(var album in listAlbum)
                    { 
                        if (record.AlbumTitle == album.AlbumTitle)
                        {
                            isExists = true;
                            break;
                        }
                            
                    }
                    /*isExists == true ? _albumRepository.Update(record) : _albumRepository.Add(record);*/
                    if (isExists)
                    {
                         record.ModifiedDate = DateTime.Now;
                        record.ModifiledBy = HttpContext.Session.GetString("UserName");
                        _albumRepository.Update(record);
                    }
                    else
                    {
                        var newRecord = new Albums
                        {
                            AlbumTitle = record.AlbumTitle,
                            ArtistId = record.ArtistId,
                            CategoryID = record.CategoryID,
                            CreatedBy = HttpContext.Session.GetString("UserName"),
                            CreatedDate = DateTime.Now,
                            Thumbnail = record.Thumbnail,
                        };
                        _albumRepository.Add(newRecord);
                    }

                }

            }
            return RedirectToAction("Index");
    }
    private bool AlbumsExists(int id)
    {
        return _albumRepository.GetById(id) != null;
    }
}
}
