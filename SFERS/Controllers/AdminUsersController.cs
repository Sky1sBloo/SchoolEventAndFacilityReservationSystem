using Microsoft.AspNetCore.Mvc;
using SFERS.Models;

namespace SFERS.Controllers
{
    public class AdminUsersController : Controller
    {
        public IActionResult Index()
        {
            var users = new List<UserProfileViewModel>
            {
                new UserProfileViewModel { Id=1, FullName="John Doe", Role="Student", IsActive=true },
                new UserProfileViewModel { Id=2, FullName="Dr. Smith", Role="Faculty", IsActive=true }
            };
            return View(users);
        }

        // Promotes/Demotes a user (e.g., Student -> Admin)
        [HttpPost]
        public IActionResult UpdateRole(int userId, string newRole)
        {
            // TODO: _userService.UpdateRole(userId, newRole);
            return RedirectToAction("Index");
        }

        // Bans or Unbans a user
        [HttpPost]
        public IActionResult ToggleBan(int userId)
        {
            // TODO: Fetch user, toggle IsActive boolean
            return RedirectToAction("Index");
        }
    }
}