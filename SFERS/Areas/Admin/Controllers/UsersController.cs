using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SFERS.Data;
using SFERS.Models.ViewModel;
using SFERS.Utilities;

namespace SFERS.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class UsersController : Controller
    {
        private ApplicationDbContext dbContext;
        public UsersController(ApplicationDbContext context)
        {
            dbContext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await GetAllUsers());
        }

        // Promotes/Demotes a user (e.g., Student -> Admin)
        [HttpPost]
        public async Task<IActionResult> UpdateRole(int userId, string newRole)
        {
            var user = await dbContext.Accounts.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var role = dbContext.Roles.FirstOrDefault(r => r.Name == newRole);
            if (role == null)
            {   
                return BadRequest("Invalid role specified: " + newRole);
            }
            user.RoleId = role.Id;
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(int userId)
        {
            var user = await dbContext.Accounts.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            user.Password = PasswordManager.HashPassword("Default@1234");
            await dbContext.SaveChangesAsync();
            TempData["Message"] = $"Password for {user.FullName} has been reset to 'Default@1234'.";
            return View("Index", await GetAllUsers());
        }

        // Bans or Unbans a user
        [HttpPost]
        public IActionResult ToggleBan(int userId)
        {
            // TODO: Fetch user, toggle IsActive boolean
            return RedirectToAction("Index");
        }

        private async Task<List<UserProfileViewModel>> GetAllUsers()
        {
            var users = await dbContext.Accounts.ToListAsync();
            var userViewModel = new List<UserProfileViewModel>();
            foreach (var user in users)
            {
                var role = await dbContext.Roles.FindAsync(user.RoleId);
                userViewModel.Add(new UserProfileViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = role != null ? role.Name : "Unknown",
                    IsActive = false
                });
            }
            return userViewModel;
        }
    }
}