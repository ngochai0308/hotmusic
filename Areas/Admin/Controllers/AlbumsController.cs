﻿using HotMusic.Contract;
using HotMusic.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotMusic.Common;
using HotMusic.DataModel;
using System.Data.Entity;
using HotMusic.Repository;
using System.Text;

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
            ViewData["filter"] = filter;

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
            string[] columnNames = new string[] { "AlbumId", "AlbumTitle", "Thumbnail", "ArtistId", "ArtistName", "CategoryID", "CategoryTitle", "CreatedDate", "CreatedBy", "ModifiedDate", "ModifiledBy" };
           
            var listAlbum = _albumRepository.GetAll();
            string csv = string.Empty;
            foreach (var column in columnNames)
            {
                csv += column + ",";
            }
            csv += "\r\n";
            foreach (var album in listAlbum)
            {
                csv += album.AlbumId.ToString().Replace(",",";")+',';
                csv += album.AlbumTitle.Replace(",", ";") + ',';
                csv += album.Thumbnail?.Replace(",", ";") + ',';
                csv += album.ArtistId.ToString().Replace(",", ";") + ',';
                csv += album.ArtistName?.Replace(",", ";") + ',';
                csv += album.CategoryID.ToString().Replace(",", ";") + ',';
                csv += album.CategoryTitle?.Replace(",", ";") + ',';
                csv += album.CreatedDate?.ToString().Replace(",", ";") + ',';
                csv += album.CreatedBy?.Replace(",", ";") + ',';
                csv += album.ModifiedDate?.ToString().Replace(",", ";") + ',';
                csv += album.ModifiledBy?.Replace(",", ";") + ','; 
                csv += "\r\n";
            }
            byte[] bytes = Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "album.csv");
        }
        private bool AlbumsExists(int id)
        {
            return _albumRepository.GetById(id) != null;
        }
    }
}
