using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFERS.Models.ViewModel;

namespace SFERS.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            var userProfile = new UserProfileViewModel 
            {
                FullName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown User",
                Role = User.FindFirstValue(ClaimTypes.Role) ?? "Invalid Role",
                Email = User.FindFirstValue(ClaimTypes.Email) ?? "",
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
