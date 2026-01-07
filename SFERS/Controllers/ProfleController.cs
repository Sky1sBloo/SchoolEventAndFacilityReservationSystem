using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFERS.Models;

namespace SFERS.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            var userProfile = new UserProfileViewModel
            {
                FullName = "John Doe",
                Role = "Student Admin",
                Email = HttpContext.Session.GetString("UserEmail") ?? "john.doe@school.edu",
                Username = "jdoe2025"
            };

            return View(userProfile);
        }

        [HttpPost]
        public IActionResult UpdatePassword(string currentPassword, string newPassword)
        {
            // Logic to update password would go here
            TempData["Message"] = "Password updated successfully!";
            return RedirectToAction("Index");
        }
    }
}
