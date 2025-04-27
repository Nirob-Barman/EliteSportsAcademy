using EliteSportsAcademy.Models.Account;
using EliteSportsAcademy.ViewModel.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EliteSportsAcademy.Controllers
{
    [Authorize(Roles="SuperAdmin,Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Dashboard()
        {
            return View();
        }
        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var roles = _roleManager.Roles.Where(r => r.Name != "SuperAdmin").ToList();
            //var userList = new List<(ApplicationUser User, IList<string> UserRoles)>();
            var userList = new List<ManageUsersViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                if (!userRoles.Contains("Admin") && !userRoles.Contains("SuperAdmin"))
                {
                    //userList.Add((user, userRoles));
                    userList.Add(new ManageUsersViewModel
                    {
                        Id = user.Id,
                        Email = user.Email!,
                        Roles = userRoles.ToList()
                    });
                }
            }

            ViewBag.Roles = roles;
            return View(userList);
        }


        [HttpPost]
        public async Task<IActionResult> ChangeUserRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(newRole))
            {
                TempData["Error"] = "Invalid user or role.";
                return RedirectToAction(nameof(ManageUsers));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove all current roles
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                TempData["Error"] = "Failed to remove current roles.";
                return RedirectToAction(nameof(ManageUsers));
            }

            // Add the new role
            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            if (!addResult.Succeeded)
            {
                TempData["Error"] = "Failed to assign new role.";
                return RedirectToAction(nameof(ManageUsers));
            }

            TempData["Success"] = "User role updated successfully!";
            return RedirectToAction(nameof(ManageUsers));
        }


    }
}
