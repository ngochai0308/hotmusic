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

namespace HotMusic.Controllers
{
    public class UsersController : Controller
    {
        private readonly MusicDbContext _context;

        public UsersController(MusicDbContext context)
        {
            _context = context;
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
            HttpContext.Session.Remove("UserName");

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
                            // Save to session
                            /*SaveUserInfoToSession(checkUser);*/
                        }

                        // Thong bao login cho authen
                        var identity = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Role, checkUser.Role) // Add column for user table => Get here
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
                            if (checkUser.Role.ToLower() == "admin")
                            {
                                return RedirectToAction("index", "home", new { Area = "Admin" });
                            }
                            return RedirectToAction("Index", "Home");
                        }
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
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(int? id, [Bind("Password", "NewPassword", "ConfirmPassword")]ChangePasswordViewModel user)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return Content("Không tìm thấy người dùng do id null");
                }
                var checkUser = _context.Users.FirstOrDefault(u=>u.UserId==id);
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
                    return RedirectToAction("index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Password", "Mật khẩu nhập không đúng");
                }
                TempData["StatusMessage"] = "Mật khẩu nhập không đúng vui lòng nhập lại!";
            }
            

            return View();
        }

        [AllowAnonymous]
        public IActionResult Accessdined()
        {
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
        public async Task<IActionResult> Register([Bind("UserName,Password,Email,ConfilmPassword")] RegisterUserViewModel user)
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
