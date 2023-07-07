using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotMusic.DataModel;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using HotMusic.Models;
using HotMusic.Common;

namespace HotMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly MusicDbContext _context;

        public CategoriesController(MusicDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index(int? PageNumber,string? keyword)
        {
            keyword ??= string.Empty;
            ViewData["currentFilter"] = keyword;
            var listCategory = (from cate in _context.Category
                     join coun in _context.Country on cate.CountryId equals coun.CountryId
                     select new Category()
                     {
                         CategoryId = cate.CategoryId,
                         CategoryTitle = cate.CategoryTitle,
                         CountryId = cate.CountryId,
                         CountryTitle = coun.CountryName,
                         CreatedBy = cate.CreatedBy,
                         CreatedDate = cate.CreatedDate,
                         ModifiledBy = cate.ModifiledBy,
                         ModifiedDate = cate.ModifiedDate,
                         IsDeleted = cate.IsDeleted
                     }).Where(ca=>ca.CategoryTitle.Contains(keyword));
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Category,CategoryDisplayViewModel>();
            }).CreateMapper();
            var listCategoryDisplay = mapper.Map<IEnumerable<CategoryDisplayViewModel>>(listCategory);
              return _context.Category != null ? 
                          View(await PaginatedList<CategoryDisplayViewModel>.CreateAsync(listCategoryDisplay,PageNumber??1,5)) :
                          Problem("Entity set 'MusicDbContext.Category'  is null.");
        }

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = (from cate in _context.Category
                            join coun in _context.Country on cate.CountryId equals coun.CountryId
                            select new Category()
                            {
                                CategoryId = cate.CategoryId,
                                CategoryTitle = cate.CategoryTitle,
                                CountryId = cate.CountryId,
                                CountryTitle = coun.CountryName,
                                CreatedBy = cate.CreatedBy,
                                CreatedDate = cate.CreatedDate,
                                ModifiledBy = cate.ModifiledBy,
                                ModifiedDate = cate.ModifiedDate,
                                IsDeleted = cate.IsDeleted
                            }).FirstOrDefault(m => m.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Category, CategoryDisplayViewModel>();
            }).CreateMapper();
            var CategoryDisplay = mapper.Map<CategoryDisplayViewModel>(category);
            return View(CategoryDisplay);
        }
        public void LoadCountrySelectList()
        {
            var listCountry = from c in _context.Country
                              select new SelectListItem()
                              {
                                  Value = c.CountryId.ToString(),
                                  Text = c.CountryName
                              };
            ViewBag.listCountry = listCountry.ToList();
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            LoadCountrySelectList();
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryTitle,CountryId")] CategoryDisplayViewModel category)
        {
            if (ModelState.IsValid)
            {
                var userName = HttpContext.Session.GetString("UserName");
                var mapper = new MapperConfiguration(config =>
                {
                    config.CreateMap<CategoryDisplayViewModel, Category>()
                    .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
                    .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => userName));
                }).CreateMapper();
                var newCategory = mapper.Map<Category>(category);
                _context.Add(newCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            LoadCountrySelectList();
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }
            LoadCountrySelectList();
            var category = (from cate in _context.Category
                            join coun in _context.Country on cate.CountryId equals coun.CountryId
                            select new Category()
                            {
                                CategoryId = cate.CategoryId,
                                CategoryTitle = cate.CategoryTitle,
                                CountryId = cate.CountryId,
                                CountryTitle = coun.CountryName,
                                CreatedBy = cate.CreatedBy,
                                CreatedDate = cate.CreatedDate,
                                ModifiledBy = cate.ModifiledBy,
                                ModifiedDate = cate.ModifiedDate,
                                IsDeleted = cate.IsDeleted
                            }).First(c=>c.CategoryId==id);
            if (category == null)
            {
                return NotFound();
            }
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Category, CategoryDisplayViewModel>();
            }).CreateMapper();
            var CategoryDisplay = mapper.Map<CategoryDisplayViewModel>(category);
            return View(CategoryDisplay);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryTitle,CountryId")] CategoryDisplayViewModel category)
        {
            if (id != category.CategoryId)
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
                        config.CreateMap<CategoryDisplayViewModel,Category>()
                         .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.Now))
                        .ForMember(dest => dest.ModifiledBy, opt => opt.MapFrom(src => userName));
                    }).CreateMapper();
                    var NewCategory = mapper.Map<Category>(category);
                    _context.Update(NewCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            LoadCountrySelectList();
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = (from cate in _context.Category
                            join coun in _context.Country on cate.CountryId equals coun.CountryId
                            select new Category()
                            {
                                CategoryId = cate.CategoryId,
                                CategoryTitle = cate.CategoryTitle,
                                CountryId = cate.CountryId,
                                CountryTitle = coun.CountryName,
                                CreatedBy = cate.CreatedBy,
                                CreatedDate = cate.CreatedDate,
                                ModifiledBy = cate.ModifiledBy,
                                ModifiedDate = cate.ModifiedDate,
                                IsDeleted = cate.IsDeleted
                            }).FirstOrDefault(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Category, CategoryDisplayViewModel>();
            }).CreateMapper();
            var CategoryDisplay = mapper.Map<CategoryDisplayViewModel>(category);
            return View(CategoryDisplay);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Category == null)
            {
                return Problem("Entity set 'MusicDbContext.Category'  is null.");
            }
            var category = await _context.Category.FindAsync(id);
            if (category != null)
            {
                _context.Category.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Category?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}
