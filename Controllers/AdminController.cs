using EliteSportsAcademy.Data;
using EliteSportsAcademy.Models.Account;
using EliteSportsAcademy.ViewModel.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EliteSportsAcademy.Controllers
{
    [Authorize(Roles="SuperAdmin,Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
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

        public async Task<IActionResult> ManageClasses()
        {
            var classes = await _context.Classes
                .Select(c => new ManageClassesViewModel
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    ClassImage = c.ClassImage,
                    InstructorName = c.InstructorName,
                    InstructorEmail = c.InstructorEmail,
                    AvailableSeats = c.AvailableSeats,
                    Price = c.Price,
                    Status = c.Status,
                    Feedback = c.Feedback,
                })
                .ToListAsync();

            return View(classes);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeClassStatus(int id, string status)
        {
            var classItem = await _context.Classes.FindAsync(id);
            if (classItem == null)
            {
                return NotFound();
            }

            classItem.Status = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Class status updated!";
            return RedirectToAction(nameof(ManageClasses));
        }

        [HttpPost]
        public async Task<IActionResult> SendFeedback(int id, string feedback)
        {
            var classItem = await _context.Classes.FindAsync(id);
            if (classItem == null)
            {
                return NotFound();
            }

            classItem.Feedback = feedback;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Feedback sent successfully!";
            return RedirectToAction(nameof(ManageClasses));
        }


    }
}
