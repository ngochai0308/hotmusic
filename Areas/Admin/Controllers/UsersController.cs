using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotMusic.Common;
using HotMusic.DataModel;
using HotMusic.Models;
using System.Security.Claims;

namespace HotMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly MusicDbContext _context;
        private readonly int _pageSize = 5;

        public UsersController(MusicDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(string filter, string sortOrder, int? pageNumber)
        {
            // Lay thong tin tu cookie
            var username = Request.Cookies["Username"];
            var password = Request.Cookies["Password"];

            // Check exist value
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                // Check login info
                var checkUser = _context.Users?.Where(u => u.UserName == username && u.Password == password).FirstOrDefault();

                if (checkUser != null)
                {
                    SaveUserInfoToSession(checkUser);
                }
            }

            // Get filter information
            if (string.IsNullOrEmpty(filter))
            {
                filter = string.Empty;
            }
            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = string.Empty;
            }
            ViewData["filter"] = filter;
            ViewData["sortFullName"] = (sortOrder == "NameASC" ? "NameDESC" : "NameASC");
            ViewData["sortUserName"] = (sortOrder == "UserNameASC" ? "UserNameDESC" : "UserNameASC");

            // List data có kiểu: Users => ứng DB
            var listData = await _context.Users.Where(u => u.FullName.Contains(filter) || u.UserName.Contains(filter)).ToListAsync();

            List<Users> listOrder;
            switch (sortOrder)
            {
                case "NameASC":
                    listOrder = listData.OrderBy(u => u.FullName).ToList();
                    break;
                case "NameDESC":
                    listOrder = listData.OrderByDescending(u => u.FullName).ToList();
                    break;
                case "UserNameASC":
                    listOrder = listData.OrderBy(u => u.UserName).ToList();
                    break;
                case "UserNameDESC":
                    listOrder = listData.OrderByDescending(u => u.UserName).ToList();
                    break;
                default:
                    listOrder = listData.OrderBy(u => u.FullName).ToList();
                    break;
            }

            /*
            // Chuyển sang list data có kiểu "UserViewModel"
            Users user1 = new Users()
            {
                Address = "test",
                Email= "test",
                FullName= "test",
                UserName    = "test"
            };

            UserViewModel userView = new UserViewModel();
            userView.Address = user1.Address;
            userView.Email = user1.Email;
            userView.FullName = user1.FullName;
            userView.UserName = user1.UserName;
            //.... toi het cac thuoc tinh


            // Su dung AutoMapper
            var mapperConfig = new AutoMapper.MapperConfiguration(config =>
            {
                config.CreateMap<Users, UserViewModel>();
            });

            var mapper = mapperConfig.CreateMapper();
            var userViewMap = mapper.Map<UserViewModel>(user1);
            */


            // Convert list
            var mapperConfig = new AutoMapper.MapperConfiguration(config =>
            {
                config.CreateMap<Users, UserViewModel>()
                .ForMember(des => des.FullName, source => source.MapFrom(s => s.Address));
            });
            var mapper = mapperConfig.CreateMapper();
            var listResult = mapper.Map<List<UserViewModel>>(listOrder);

            return _context.Users != null ?
                        View(await PaginatedList<UserViewModel>.CreateAsync(listResult, pageNumber ?? 1, _pageSize)) :
                        Problem("Entity set 'MusicDbContext.Users'  is null.");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (users == null)
            {
                return NotFound();
            }

            // Convert to USerViewModel before show on UI
            var mapperConfig = new AutoMapper.MapperConfiguration(config =>
            {
                config.CreateMap<Users, UserViewModel>();
            });
            var mapper = mapperConfig.CreateMapper();
            var displayUser = mapper.Map<UserViewModel>(users);

            return View(displayUser);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FullName,Password,UserName,Email,PhoneNumber,Address")] UserViewModel insertUser)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Mapping to Users model  of database
                    var mapperConfig = new AutoMapper.MapperConfiguration(config =>
                    {
                        config.CreateMap<UserViewModel, Users>();
                    });

                    var mapper = mapperConfig.CreateMapper();
                    var newUser = mapper.Map<Users>(insertUser);

                    _context.Add(newUser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Create user error: {ex.Message}");
                }
            }
            return View(insertUser);
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            // Save return param
            var returnParam = HttpContext.Request.Query["successReturnUrl"];
            if (!string.IsNullOrEmpty(returnParam))
            {
                HttpContext.Session.SetString("returnUrl", returnParam);
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            // Delete session
            HttpContext.Session.Remove("FullName");
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("PhoneNumber");

            // Remove authen
            await HttpContext.SignOutAsync();

            // Redirect to login page
            return RedirectToAction(nameof(Login));
        }

        /// <summary>
        /// Save user information into session
        /// </summary>
        /// <param name="user"></param>

        private void SaveUserInfoToSession(Users user)
        {
            HttpContext.Session.SetString("UserName", user.UserName);
        }

        /// <summary>
        ///  Save user information into cookies
        /// </summary>
        /// <param name="user"></param>
        private void SaveUserInfoToCookie(Users user)
        {
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(1);
            cookieOptions.Path = "/";
            cookieOptions.Secure = true;

            // Write value into response
            Response.Cookies.Append("UserName", user.UserName, cookieOptions);
            Response.Cookies.Append("Password", user.Password, cookieOptions);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([Bind("UserName, Password, IsRememberMe")] UserViewModel user)
        {
            if (!string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.Password))
            {
                // Check data in DB
                var checkUser = _context.Users.Where(u =>
                                           u.UserName == user.UserName
                                        && u.Password == user.Password).FirstOrDefault();
                if (checkUser != null)
                {
                    // Save to session
                    SaveUserInfoToSession(checkUser);

                    // Cookie
                    /*if (user.IsRememberMe)
                    {
                        // Save to cookie
                        var cookieOption = new CookieOptions();
                        cookieOption.Expires = DateTime.Now.AddMonths(1); // Luu mot thang
                        cookieOption.Path = "/";
                        cookieOption.Secure = true;

                        Response.Cookies.Append("Username", user.UserName, cookieOption);
                        Response.Cookies.Append("Password", user.Password, cookieOption);
                    }*/

                    // Thong bao login cho authen
                    var identity = new ClaimsIdentity(new[] 
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, "User") // Add column for user table => Get here
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    var claimPrincipal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal);

                    // Get return url from session
                    var returnUrl = HttpContext.Session.GetString("returnUrl");
                    if(!string.IsNullOrEmpty(returnUrl))
                    {
                        Response.Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    //ViewData["Error"] = "Tên đăng nhập hoặc mật khẩu không đúng!";
                    ModelState.AddModelError("UserName", "Tên đăng nhập hoặc mật khẩu không đúng!");

                    ModelState.AddModelError("", "1.Tên đăng nhập hoặc mật khẩu không đúng!");
                    ModelState.AddModelError("", "2.Tên đăng nhập hoặc mật khẩu không đúng!");

                }
            }
            else
            {
                ViewData["Error"] = "Vui lòng nhập tên đăng nhập và mật khẩu!";
                ModelState.AddModelError("", "Vui lòng nhập tên đăng nhập và mật khẩu!");
            }

            // Ở nguyên trang hiện tại
            return View(user);
        }

        [AllowAnonymous]
        public IActionResult Accessdined()
        {
            return View();
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FullName,Password,UserName,Email,PhoneNumber,Address")] UserViewModel users)
        {
            if (id != users.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var mapperConfig = new AutoMapper.MapperConfiguration(config =>
                    {
                        config.CreateMap<UserViewModel, Users>();
                    });
                    var mapper = mapperConfig.CreateMapper();
                    var updateUser = mapper.Map<Users>(users);

                    _context.Update(updateUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.UserId))
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
            return View(users);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'MusicDbContext.Users'  is null.");
            }
            var users = await _context.Users.FindAsync(id);
            if (users != null)
            {
                _context.Users.Remove(users);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
