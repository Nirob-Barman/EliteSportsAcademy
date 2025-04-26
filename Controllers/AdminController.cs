using EliteSportsAcademy.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EliteSportsAcademy.Controllers
{
    [Authorize(Roles="SuperAdmin,Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Dashboard()
        {
            return View();
        }
        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var filteredUsers = new List<ApplicationUser>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin") && !roles.Contains("SuperAdmin"))
                {
                    filteredUsers.Add(user);
                }
            }
            return View(filteredUsers);
        }


        [HttpPost]
        public async Task<IActionResult> ChangeUserRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Get current roles of the user
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove the old roles (if any)
            foreach (var role in currentRoles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            // Add the new role
            var result = await _userManager.AddToRoleAsync(user, newRole);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ManageUsers));
            }

            // If adding role failed, you can handle it here (optional)
            TempData["Error"] = "Role change failed!";
            return RedirectToAction(nameof(ManageUsers));
        }


    }
}
