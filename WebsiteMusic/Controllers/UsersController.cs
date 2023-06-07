using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebsiteMusic.DataModel;
using WebsiteMusic.Models;
using System.IO;

namespace WebsiteMusic.Controllers
{
    public class UsersController : Controller
    {
        private readonly MusicDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [TempData] public string StatusMessage { get; set; }

        public UsersController(MusicDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
              return _context.User != null ? 
                          View(await _context.User.ToListAsync()) :
                          Problem("Entity set 'MusicDBContext.User'  is null.");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Profile(string id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserName,Password,Email,ConfilmPassword")] RegisterUserViewModel user)
        {
            if(await _context.User.AnyAsync(u => u.UserName == user.UserName))
            {
                ModelState.AddModelError("UserName", "Tên tài khoản đã tồn tại.Vui lòng đổi tên khác!");
            }
            if(await _context.User.AnyAsync(u=>u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.Vui lòng đổi email khác!");
            }
            if (ModelState.IsValid)
            {
                var mapper = new MapperConfiguration(config =>
                {
                    config.CreateMap<RegisterUserViewModel, User>()
                    .ForMember(u => u.UserId, ru => ru.MapFrom(sr => Guid.NewGuid()))
                    .ForMember(u => u.UserName, ru => ru.MapFrom(sr => sr.UserName))
                    .ForMember(u => u.Password, ru => ru.MapFrom(sr => Common.Hash.HashPassword(user.Password)))
                    .ForMember(u => u.Email, ru => ru.MapFrom(sr => sr.Email));
                }).CreateMapper();
                var newU = mapper.Map<User>(user);

                _context.User.Add(newU);
                await _context.SaveChangesAsync();
                var cookieOption = new CookieOptions();
                cookieOption.Expires = DateTime.Now.AddMonths(1);
                cookieOption.Path = "/";
                cookieOption.Secure = true;

                Response.Cookies.Append("UserName", user.UserName, cookieOption);
                Response.Cookies.Append("Password", user.Password, cookieOption);
                TempData["StatusMessage"] = "Bạn đã đăng kí thành công!!";
                return RedirectToAction("Login");
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<User, EditUserViewModel>();
            }).CreateMapper();
            var displayUser = mapper.Map<EditUserViewModel>(user);
            if (user == null)
            {
                return NotFound();
            }
            return View(displayUser);  
        }
        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,UserName,FileUpload,Password,DateOfBirth,Email,FullName,PhoneNumber,Gender,Address")] EditUserViewModel user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            { 
                try
                {
                    var checkUser = _context.User.First(u => u.UserId == user.UserId);
                    var oldFileName = checkUser.image;
                    var file1 = oldFileName;

                    if(user.FileUpload != null)
                    {
                        file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                        + Path.GetExtension(user.FileUpload.FileName);
                        var file = Path.Combine("wwwroot", "image", "User", file1);
                        using var filestream = new FileStream(file, FileMode.Create);
                        await user.FileUpload.CopyToAsync(filestream);
                    }         
                    var mapper = new MapperConfiguration(configure => {
                        configure.CreateMap<EditUserViewModel, User>();
                    }).CreateMapper();
                    checkUser = mapper.Map<User>(user);
                    
                    if (!string.IsNullOrEmpty(oldFileName) && oldFileName != file1)
                    {
                        var fileOld = Path.Combine("wwwroot", "image", "User", oldFileName);
                        if(System.IO.File.Exists(fileOld))
                           System.IO.File.Delete(fileOld);
                    }
                    
                    checkUser.image = file1;
                    HttpContext.Session.SetString("Image", file1);

                    var trackedUser = _context.User.FirstOrDefault(u => u.UserId == user.UserId);
                    if (trackedUser != null)
                        _context.Entry(trackedUser).State = EntityState.Detached;

                    _context.Entry(checkUser).State = EntityState.Modified;

                    _context.Update(checkUser);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Profile),new {id = user.UserId});
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'MusicDBContext.User'  is null.");
            }
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login([Bind("UserName,Password,IsRememberMe")]LoginUserViewModel user)
        {
            
            if (ModelState.IsValid)
            {
                var checkUser = _context.User.FirstOrDefault(u=>u.UserName == user.UserName);
                if(checkUser  != null)
                {
                    if(Common.Hash.VerifyPassword(user.Password, checkUser.Password))
                    {
                        if (user.IsRememberMe)
                        {
                            var cookieOption = new CookieOptions();
                            cookieOption.Expires = DateTime.Now.AddMonths(1);
                            cookieOption.Path = "/";
                            cookieOption.Secure = true;

                            Response.Cookies.Append("UserName", user.UserName, cookieOption);
                            Response.Cookies.Append("Password", user.Password, cookieOption);
                        }
                        else
                        {
                            Response.Cookies.Delete("UserName");
                            Response.Cookies.Delete("Password");
                        }
                        HttpContext.Session.SetString("UserName", checkUser.UserName);
                        HttpContext.Session.SetString("Id", checkUser.UserId);
                        HttpContext.Session.SetString("Image", checkUser.image);
                        TempData["StatusMessage"] = "Bạn đã đăng nhập thành công";
                        return RedirectToAction("Index", "Home");
                    }
                }
                TempData["StatusMessage"] = "Sai tên tài khoản hoặc mật khẩu! Vui lòng nhập lại.";
                return View();
            }
            return View();
        }
        public IActionResult Logout() {
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("Id");
            TempData["StatusMessage"] = "Bạn đã đăng xuất";
            return RedirectToAction("Index", "Home");
        }
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string id, [Bind("Password,NewPassword,ConfilmPassword")] ChangePasswordViewModel user)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return Content("Không tìm thấy người dùng do id null");
                }
                var checkUser = _context.User.First(u => u.UserId == id);
                if (checkUser == null)
                {
                    return Content("Không tìm thấy người dùng");
                }
                if (Common.Hash.VerifyPassword(user.Password, checkUser.Password))
                {
                    checkUser.Password = Common.Hash.HashPassword(user.NewPassword);
                    _context.SaveChanges();
                    Response.Cookies.Append("Password", user.NewPassword);
                    TempData["StatusMessage"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("index", "Home");
                }
                TempData["StatusMessage"] = "Mật khẩu nhập không đúng vui lòng nhập lại!";
            }

            return View();
        }
        public IActionResult FogotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> FogotPassword(string email)
        {
            if (ModelState.IsValid)
            {
                var checkUser = _context.User.First(User => User.Email == email);
                if(checkUser == null)
                {
                    TempData["StatusMessage"] = "Email không tồn tại trên hệ thống. Vui lòng nhập lại!!!";
                    return View();
                }
                var link = $"<a href=\"https://localhost:7272/resetpassword?email={email}\">Click here to reset your password</a>";
                var body = $"Bạn vui lòng click vào trang này để thay đổi mật khẩu: {link}";
                await Common.MailsUtils.SendGmail("Hai0xautrai@gmail.com", email, "Lấy lại mật khẩu cho Website Music", body);
                return View("Check");
            }
            
            return View();
        }
        [HttpGet("/resetPassword")]
        public IActionResult ResetPassword()
        {
            var returnParam = HttpContext.Request.Query["email"];
            if(!string.IsNullOrEmpty(returnParam))
            {
                HttpContext.Session.SetString("email", returnParam);
            }
            return View();
        }
        [HttpPost("/resetPassword")]
        public IActionResult ResetPassword([Bind("Password,ConfilmPassword")] ResetPasswordViewModel user)
        {
            if (ModelState.IsValid)
            {
                string email = HttpContext.Session.GetString("email");
                if (string.IsNullOrEmpty(email))
                {
                    return Content("Không tìm thấy email");
                }
                var checkUser = _context.User.First(User=>User.Email == email);
                if( checkUser == null)
                {
                    return Content("Không tìm thấy email");
                }
                checkUser.Password = Common.Hash.HashPassword(user.Password);
                _context.SaveChanges();
                TempData["StatusMessage"] = "Bạn đã thay đổi mật khẩu thành công";
                return View("Login");
            }
            return View();
        }
        private bool UserExists(string id)
        {
          return (_context.User?.Any(e => e.UserId == id)).GetValueOrDefault();
        }

    }
}
