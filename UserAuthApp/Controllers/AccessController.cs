using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using UserAuthApp.Data;
using UserAuthApp.Models;
using UserAuthApp.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UserAuthApp.Controllers
{
    public class AccessController : Controller
    {
        private readonly AppDBContext _appDbContext;
        private readonly PasswordHasher<User> _passwordHasher;
        public AccessController(AppDBContext appDBContext)
        {
            _appDbContext = appDBContext;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ViewData["Message"] = "Password and Confirm password are not the same";
                return View();
            }

            User? userSearched = await _appDbContext.Users.Where(u => u.Email == model.Email).FirstOrDefaultAsync();

            if (userSearched != null)
            {
                ViewData["Message"] = "Verificate email or password";
                return View();
            }

            User user = new User()
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = _passwordHasher.HashPassword(null, model.Password)
            };

            await _appDbContext.Users.AddAsync(user);
            await _appDbContext.SaveChangesAsync();

            if (user.IdUser != 0) return RedirectToAction("Login", "Access");

            ViewData["Message"] = "Something was wrong...";
            return View();

        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User? userSearched = await _appDbContext.Users
                                    .Where(user =>
                                    user.Email == model.Email).FirstOrDefaultAsync();

            if (userSearched == null ||
                _passwordHasher.VerifyHashedPassword(userSearched, userSearched.Password, model.Password) != PasswordVerificationResult.Success)
            {
                ViewData["Message"] = "Verificate email or password";
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userSearched.FullName)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            return RedirectToAction("Index", "Home");
        }
    }
}