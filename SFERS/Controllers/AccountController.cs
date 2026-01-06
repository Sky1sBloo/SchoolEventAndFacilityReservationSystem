using Microsoft.AspNetCore.Mvc;
using SFERS.Models;

namespace SFERS.Controllers
{
    public class AccountController : Controller
    {
        private static Dictionary<string, string> RegisteredUsers = new Dictionary<string, string>();

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

            if (model.Password.Length < 6)
            {
                ModelState.AddModelError("Password", "Password must be at least 6 characters.");
                return View(model);
            }

            if (!RegisteredUsers.ContainsKey(model.Email.ToLower()))
            {
                ModelState.AddModelError("", "No account found with this email. Please register first.");
                return View(model);
            }

            if (RegisteredUsers[model.Email.ToLower()] != model.Password)
            {
                ModelState.AddModelError("", "Invalid email or password. Please check your credentials.");
                return View(model);
            }

            // Set session to track logged-in user
            HttpContext.Session.SetString("IsLoggedIn", "true");
            HttpContext.Session.SetString("UserEmail", model.Email);

            return RedirectToAction("Index", "Dashboard");
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

            if (RegisteredUsers.ContainsKey(model.Email.ToLower()))
            {
                ModelState.AddModelError("Email", "This email is already registered. Please login or use a different email.");
                return View(model);
            }

            if (model.Password.Length < 6)
            {
                ModelState.AddModelError("Password", "Password must be at least 6 characters long.");
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match. Please try again.");
                return View(model);
            }

            RegisteredUsers[model.Email.ToLower()] = model.Password;

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
