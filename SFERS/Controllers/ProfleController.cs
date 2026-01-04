using Microsoft.AspNetCore.Mvc;
using SFERS.Models;

namespace SFERS.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            // MOCK DATA: Simulate the currently logged-in user
            var userProfile = new UserProfileViewModel
            {
                FullName = "John Doe",
                Role = "Student Admin",
                Email = "john.doe@school.edu",
                Username = "jdoe2025"
            };

            return View(userProfile);
        }

        [HttpPost]
        public IActionResult UpdatePassword(string currentPassword, string newPassword)
        {
            // Logic to update password would go here
            // _authService.ChangePassword(User.Identity.Name, currentPassword, newPassword);

            TempData["Message"] = "Password updated successfully!";
            return RedirectToAction("Index");
        }
    }
}