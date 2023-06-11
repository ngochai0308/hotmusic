using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotMusic.Common;
using HotMusic.DataModel;
using HotMusic.Models;
using System.Security.Claims;

namespace HotMusic.Controllers
{
    [Authorize]
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
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("PhoneNumber", user.PhoneNumber);
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

        private bool UsersExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
