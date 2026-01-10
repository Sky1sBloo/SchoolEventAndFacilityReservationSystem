using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFERS.Data;
using SFERS.Models.ViewModel;

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
        public IActionResult Index()
        {
            var users = dbContext.Accounts.ToList();
            var userViewModel = new List<UserProfileViewModel>();
            foreach (var user in users)
            {
                var role = dbContext.Roles.Find(user.RoleId);
                userViewModel.Add(new UserProfileViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = role != null ? role.Name : "Unknown",
                    IsActive = false
                });
            }
            return View(userViewModel);
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

        // Bans or Unbans a user
        [HttpPost]
        public IActionResult ToggleBan(int userId)
        {
            // TODO: Fetch user, toggle IsActive boolean
            return RedirectToAction("Index");
        }
    }
}