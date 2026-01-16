using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models.ViewModel;
using SFERS.Utilities;

namespace SFERS.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private ApplicationDbContext dbContext;
        public ProfileController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            return View(GetUserProfile());
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string currentPassword, string newPassword, string confirmNewPassword)
        {
            var account = await dbContext.Accounts
                .FirstOrDefaultAsync(a => a.Id.ToString() == User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            if (account == null)
            {
                ModelState.AddModelError("", "User account not found.");
                return View("Index", GetUserProfile());
            }
            if (!PasswordManager.VerifyPassword(currentPassword, account.Password))
            {
                ModelState.AddModelError("", "Current password is incorrect.");
                return View("Index", GetUserProfile());
            }
            if (newPassword != confirmNewPassword)
            {
                ModelState.AddModelError("", "New password and confirmation do not match.");
                return View("Index", GetUserProfile());
            }
            account.Password = PasswordManager.HashPassword(newPassword);
            await dbContext.SaveChangesAsync();
            TempData["Message"] = "Password updated successfully!";
            return RedirectToAction("Index");
        }

        private UserProfileViewModel GetUserProfile()
        {
            return new UserProfileViewModel 
            {
                FullName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown User",
                Role = User.FindFirstValue(ClaimTypes.Role) ?? "Invalid Role",
                Email = User.FindFirstValue(ClaimTypes.Email) ?? "",
            };
        }
    }
}
