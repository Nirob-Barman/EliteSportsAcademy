using EliteSportsAcademy.Data;
using EliteSportsAcademy.Models.Account;
using EliteSportsAcademy.Models.Student;
using EliteSportsAcademy.ViewModel.Instructor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EliteSportsAcademy.Controllers
{
    public class ClassController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClassController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            //if (user == null) return RedirectToAction("Login", "Account");
            if(user == null)
            {
                var xclasses = await _context.Classes
                .Where(c => c.Status == "approved") // show only approved classes
                .Include(c => c.Instructor)
                .Select(c => new ClassViewModel
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    ClassImagePath = c.ClassImage,
                    //InstructorName = c.InstructorName,
                    //InstructorEmail = c.InstructorEmail,
                    InstructorName = c.Instructor != null ? $"{c.Instructor.FirstName} {c.Instructor.LastName}" : "N/A",
                    AvailableSeats = c.AvailableSeats,
                    Price = c.Price,
                    Status = c.Status,
                    Feedback = c.Feedback
                })
                .ToListAsync();

                return View(xclasses);
            }

            
            //Get all class IDs the user has selected but not enrolled yet:
            var selectedClassIds = await _context.SelectedClasses
                .Where(sc => sc.StudentId == user.Id)
                .Select(sc => sc.ClassId)
                .ToListAsync();

            //Get all class IDs the user is enrolled in:
            var enrolledClassIds = await _context.EnrolledClasses
                .Where(ec => ec.StudentId == user.Id)
                .Select(ec => ec.ClassId)
                .ToListAsync();

            //Combine both lists into a single set:
            //var combinedClassIds = selectedClassIds.Union(enrolledClassIds).ToHashSet();

            var classes = await _context.Classes
                .Where(c => c.Status == "approved") // show only approved classes
                .Include(c => c.Instructor)
                .Select(c => new ClassViewModel
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    ClassImagePath = c.ClassImage,
                    //InstructorName = c.InstructorName,
                    //InstructorEmail = c.InstructorEmail,
                    InstructorName = c.Instructor != null ? $"{c.Instructor.FirstName} {c.Instructor.LastName}" : "N/A",
                    AvailableSeats = c.AvailableSeats,
                    Price = c.Price,
                    Status = c.Status,
                    Feedback = c.Feedback,
                    //IsSelected = selectedClassIds.Contains(c.Id)
                    //IsSelected = combinedClassIds.Contains(c.Id)
                    IsSelected = selectedClassIds.Contains(c.Id),
                    IsEnrolled = enrolledClassIds.Contains(c.Id)
                })
                .ToListAsync();

            return View(classes);
        }
    }
}
