using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotMusic.Common;
using HotMusic.DataModel;
using HotMusic.Models;
using System.Security.Claims;
using AutoMapper;
using System.Security.Policy;
using WebsiteMusic.Models;
using HotMusic.Contract;

namespace HotMusic.Controllers
{
    public class UsersController : Controller
    {
        private readonly MusicDbContext _context;
        private readonly IUserRepository _userRepository;

        public UsersController(MusicDbContext context,IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
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
        public IActionResult Profile(int id)
        {
            var user = _context.Users.First(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _context == null)
            {
                return NotFound();
            }

            var user = _context.Users.First(u => u.UserId == id);
            var mapper = new MapperConfiguration(config =>
            {
                config.CreateMap<Users, EditUserViewModel>();
            }).CreateMapper();
            var displayUser = mapper.Map<EditUserViewModel>(user);
            if (user == null)
            {
                return NotFound();
            }
            return View(displayUser);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserName,FileUpload,Password,DateOfBirth,Email,FullName,PhoneNumber,Gender,Address")] EditUserViewModel user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {

                var checkUser = _userRepository.GetById(id);
                var oldFileName = checkUser.image != null ? checkUser.image : string.Empty;
                var file1 = oldFileName;

                if (user.FileUpload != null)
                {
                    file1 = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                    + Path.GetExtension(user.FileUpload.FileName);
                    var file = Path.Combine("wwwroot", "img", "User", file1);
                    using var filestream = new FileStream(file, FileMode.Create);
                    await user.FileUpload.CopyToAsync(filestream);
                }
                //Xoa file anh cu neu co
                if (!string.IsNullOrEmpty(oldFileName) && oldFileName != file1)
                {
                    var fileOld = Path.Combine("wwwroot", "img", "User", oldFileName);
                    if (System.IO.File.Exists(fileOld))
                        System.IO.File.Delete(fileOld);
                }
                HttpContext.Session.SetString("Image", file1);

                var mapper = new MapperConfiguration(configure =>
                {
                    configure.CreateMap<EditUserViewModel, Users>()
                    .ForMember(dest=>dest.image,opt => opt.MapFrom(src=>file1));
                }).CreateMapper();
                var updateUser = mapper.Map<Users>(user);

                _userRepository.CheckTrackedUser(id);

                _userRepository.Update(updateUser);
                return RedirectToAction(nameof(Profile), new { id = user.UserId });
            }
            return View(user);
        }
        public async Task<IActionResult> Logout()
        {
            // Delete session
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("Role");
            HttpContext.Session.Remove("Id");

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
            HttpContext.Session.SetInt32("Id", user.UserId);
            HttpContext.Session.SetString("Image", user.image??"");
            HttpContext.Session.SetString("Role", user.Role ?? "User");
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
                                           u.UserName == user.UserName).FirstOrDefault();

                if (checkUser != null)
                {
                    if (HashPass.VerifyPassword(user.Password, checkUser.Password))
                    {
                        SaveUserInfoToSession(checkUser);

                        // Cookie
                        if (user.IsRememberMe)
                        {
                            // Save to cookie
                            var cookieOption = new CookieOptions();
                            cookieOption.Expires = DateTime.Now.AddMonths(1); // Luu mot thang
                            cookieOption.Path = "/";
                            cookieOption.Secure = true;

                            Response.Cookies.Append("Username", user.UserName, cookieOption);
                            Response.Cookies.Append("Password", user.Password, cookieOption);
                        }
                        var role = checkUser.Role ?? "User";
                        // Thong bao login cho authen
                        var identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Role, role) // Add column for user table => Get here
                        }, CookieAuthenticationDefaults.AuthenticationScheme);

                        var claimPrincipal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal);

                        // Get return url from session
                        var returnUrl = HttpContext.Session.GetString("returnUrl");
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            Response.Redirect(returnUrl);
                        }
                        else
                        {
                            if (checkUser.Role?.ToLower() == "admin")
                            {
                                return RedirectToAction("index", "home", new { Area = "Admin" });
                            }
                            return RedirectToAction("HomePage", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "Tên đăng nhập hoặc mật khẩu không đúng!");
                    }
                }
                else
                {
                    //ViewData["Error"] = "Tên đăng nhập hoặc mật khẩu không đúng!";
                    ModelState.AddModelError("UserName", "Tên đăng nhập hoặc mật khẩu không đúng!");
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
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(int? id, [Bind("Password", "NewPassword", "ConfirmPassword")] ChangePasswordViewModel user)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return Content("Không tìm thấy người dùng do id null");
                }
                var checkUser = _context.Users.FirstOrDefault(u => u.UserId == id);
                if (checkUser == null)
                {
                    return Content("Không tìm thấy người dùng");
                }
                if (HashPass.VerifyPassword(user.Password, checkUser.Password))
                {
                    checkUser.Password = Common.HashPass.HashPassword(user.NewPassword);
                    _context.SaveChanges();
                    Response.Cookies.Append("Password", user.NewPassword);
                    TempData["StatusMessage"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("HomePage", "Home");
                }
                else
                {
                    ModelState.AddModelError("Password", "Mật khẩu nhập không đúng");
                }
            }


            return View();
        }
        [AllowAnonymous]
        public IActionResult FogotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Accessdined()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> FogotPassword(string email)
        {
            if (ModelState.IsValid)
            {
                if (!_context.Users.Any(u => u.Email == email))
                {
                    ModelState.AddModelError("email", "Email bạn nhập không tồn tại trên hệ thống!");
                    TempData["StatusMessage"] = "Email bạn nhập không tồn tại trên hệ thống!";
                    return View();
                }
                var link = $"<a href=\"https://musicweb.online/resetpassword?email={email}\">Click here to reset your password</a>";
                var body = $"Bạn vui lòng click vào trang này để thay đổi mật khẩu: {link}";
                await MailsUtils.SendGmail("Hai0xautrai@gmail.com", email, "Lấy lại mật khẩu cho Website Music", body);
                return View("Check");
            }

            return View();
        }
        [AllowAnonymous]
        [HttpGet("/resetPassword")]
        public IActionResult ResetPassword()
        {
            var returnParam = HttpContext.Request.Query["email"];
            if (!string.IsNullOrEmpty(returnParam))
            {
                HttpContext.Session.SetString("email", returnParam);
            }
            return View();
        }
        [AllowAnonymous]
        [HttpPost("/resetPassword")]
        public IActionResult ResetPassword([Bind("NewPassword,ConfirmPassword")] ResetPasswordViewModel user)
        {
            if (ModelState.IsValid)
            {
                string email = HttpContext.Session.GetString("email");
                if (string.IsNullOrEmpty(email))
                {
                    return Content("Không tìm thấy email");
                }
                var checkUser = _context.Users.First(u => u.Email == email);
                if (checkUser == null)
                {
                    return Content("Không tìm thấy người dùng");
                }
                checkUser.Password = HashPass.HashPassword(user.NewPassword);
                _context.SaveChanges();
                TempData["StatusMessage"] = "Bạn đã thay đổi mật khẩu thành công";
                return View("Login");
            }
            return View();
        }
        private bool UsersExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register([Bind("UserName,Password,Email,ConfirmPassword")] RegisterUserViewModel user)
        {
            if (user.UserName != null && user.Email != null)
            {
                if (_context.Users.Any(u => u.UserName == user.UserName))
                {
                    ModelState.AddModelError("UserName", "Tên tài khoản đã tồn tại.Vui lòng đổi tên khác!");
                }
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại.Vui lòng đổi email khác!");
                }
            }

            if (ModelState.IsValid)
            {
                var mapper = new MapperConfiguration(config =>
                {
                    config.CreateMap<RegisterUserViewModel, Users>()
                    .ForMember(u => u.UserName, ru => ru.MapFrom(sr => sr.UserName))
                    .ForMember(u => u.Password, ru => ru.MapFrom(sr => Common.HashPass.HashPassword(user.Password)))
                    .ForMember(u => u.Email, ru => ru.MapFrom(sr => sr.Email));
                }).CreateMapper();
                var newU = mapper.Map<Users>(user);

                _context.Add(newU);
                _context.SaveChanges();
                var cookieOption = new CookieOptions();
                cookieOption.Expires = DateTime.Now.AddMonths(1);
                cookieOption.Path = "/";
                cookieOption.Secure = true;

                Response.Cookies.Append("UserName", user.UserName, cookieOption);
                Response.Cookies.Append("Password", user.Password, cookieOption);
                return RedirectToAction("Login");
            }
            return View(user);
        }
    }
}
