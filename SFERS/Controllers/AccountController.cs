using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models;
using SFERS.Models.Entities;
using SFERS.Utilities;

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
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", "Please enter both email and password.");
                return View(model);
            }

            if (!model.Email.Contains("@") || !model.Email.Contains("."))
            {
                ModelState.AddModelError("Email", "Please enter a valid email address.");
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Password) || model.Password.Length < 6)
            {
                ModelState.AddModelError("Password", "Password must be at least 6 characters.");
                return View(model);
            }

            // Set session to track logged-in user
            var account = dbContext.Accounts.Include(a => a.Role)
                .FirstOrDefault(a => a.Username == model.Email);

            if (account != null && PasswordManager.VerifyPassword(model.Password, account.Password))
            {
                HttpContext.Session.SetString("UserRole", account.Role.Name);
                return RedirectToAction("Index", "Dashboard");
            }
            ModelState.AddModelError("", "Invalid email or password. Please check your credentials.");
            return View(model);
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.FullName))
            {
                ModelState.AddModelError("FullName", "Please enter your full name.");
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Email) || !model.Email.Contains("@") || !model.Email.Contains("."))
            {
                ModelState.AddModelError("Email", "Please enter a valid email address (e.g., name@school.edu).");
                return View(model);
            }

            if (string.IsNullOrEmpty(model?.Password) || model.Password.Length < 6)
            {
                ModelState.AddModelError("Password", "Password must be at least 6 characters long.");
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match. Please try again.");
                return View(model);
            }
            var defaultRole = dbContext.Roles.FirstOrDefault(r => r != null && r.Name == "Student",
                null) ?? new Role { Name = "Role", Description = "User role" };

            var account = new Account
            {
                Username = model.Email,
                Password = PasswordManager.HashPassword(model.Password),
                RoleId = defaultRole.Id,
                Role = defaultRole,
                Email = model.Email,
                FullName = model.FullName
            };
            dbContext.Accounts.Add(account);
            dbContext.SaveChanges();

            TempData["Message"] = "Registration successful! Please login with your credentials.";
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
