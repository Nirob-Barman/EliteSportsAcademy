using System.Security.Claims;
using EliteSportsAcademy.Data;
using EliteSportsAcademy.Models.Account;
using EliteSportsAcademy.Models.Instructor;
using EliteSportsAcademy.ViewModel.Instructor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EliteSportsAcademy.Controllers
{
    [Authorize]
    public class InstructorController : Controller
    {
        private readonly AppDbContext _context;
        public InstructorController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        // GET: Add Class Form
        public IActionResult AddClass()
        {
            return View();
        }
        // POST: Add Class Submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClass(ClassViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Map ViewModel to Entity (if needed)
                var newClass = new Class
                {
                    ClassName = model.ClassName,
                    ClassImage = model.ClassImage,
                    InstructorName = model.InstructorName,
                    InstructorEmail = model.InstructorEmail,
                    AvailableSeats = model.AvailableSeats,
                    Price = model.Price
                };

                // Save to database
                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Class added successfully!";
                return RedirectToAction(nameof(AddClass));
                //return RedirectToAction(nameof(MyClasses));
            }

            // If validation fails, redisplay the form
            return View(model);
        }

        // GET: View My Classes
        [HttpGet]
        public IActionResult MyClasses()
        {
            // Find the current instructor's email
            var instructorEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(instructorEmail))
            {
                return RedirectToAction("Dashboard");
            }

            // Fetch classes where InstructorEmail matches
            var myClasses = _context.Classes
                .Where(c => c.InstructorEmail == instructorEmail)
                .Select(c => new ClassViewModel
                {
                    ClassName = c.ClassName,
                    ClassImage = c.ClassImage,
                    InstructorName = c.InstructorName,
                    InstructorEmail = c.InstructorEmail,
                    AvailableSeats = c.AvailableSeats,
                    Price = c.Price,
                    Status = c.Status,
                    Feedback = c.Feedback
                })
                .ToList();

            return View(myClasses);
        }




    }
}
