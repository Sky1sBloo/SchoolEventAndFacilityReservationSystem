using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models;
using SFERS.Models.Entities;
using SFERS.Utilities;
using System.Security.Claims;

namespace SFERS.Controllers
{

    public class AccountController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public AccountController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var account = dbContext.Accounts.Include(a => a.Role)
                .FirstOrDefault(a => a.Email == model.Email);

            if (account != null && PasswordManager.VerifyPassword(model.Password, account.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                    new Claim(ClaimTypes.Email, account.Email),
                    new Claim(ClaimTypes.Name, account.FullName),
                    new Claim(ClaimTypes.Role, account.Role.Name)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Dashboard");
            }
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match. Please try again.");
                return View(model);
            }
            var defaultRole = dbContext.Roles.FirstOrDefault(r => r.Name == "User");
            if (defaultRole == null)
            {
                ModelState.AddModelError("", "System error: Default role not found.");
                return View(model);
            }

            var account = new Account
            {
                Email = model.Email,
                FullName = model.FullName,
                Password = PasswordManager.HashPassword(model.Password),
                RoleId = defaultRole.Id,
                Role = defaultRole
            };
            await dbContext.Accounts.AddAsync(account);
            dbContext.SaveChanges();

            TempData["Message"] = "Registration successful! Please login with your credentials.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
