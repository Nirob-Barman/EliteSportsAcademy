using EliteSportsAcademy.Data;
using EliteSportsAcademy.Models.Account;
using EliteSportsAcademy.Models.Student;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EliteSportsAcademy.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public StudentController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        //public async Task<IActionResult> Dashboard()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null) return RedirectToAction("Login", "Account");

        //    var selectedClasses = await _context.SelectedClasses
        //        .Include(s => s.Class)
        //        .Where(s => s.StudentId == user.Id)
        //        .ToListAsync();

        //    var enrolledClasses = await _context.EnrolledClasses
        //        .Include(e => e.Class)
        //        .Where(e => e.StudentId == user.Id)
        //        .ToListAsync();

        //    ViewBag.SelectedClasses = selectedClasses;
        //    ViewBag.EnrolledClasses = enrolledClasses;

        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> SelectClass(int classId)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null) return Unauthorized();

        //    var selectedClass = new SelectedClass
        //    {
        //        ClassId = classId,
        //        StudentId = user.Id
        //    };

        //    _context.SelectedClasses.Add(selectedClass);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Dashboard");
        //}


        public async Task<IActionResult> EnrolledClasses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var enrolledClasses = await _context.EnrolledClasses
                .Include(e => e.Class)
                .Where(e => e.StudentId == user.Id)
                .ToListAsync();

            return View(enrolledClasses);
        }

        [HttpGet]
        public async Task<IActionResult> SelectedClasses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var selectedClasses = await _context.SelectedClasses
                .Include(s => s.Class)
                .Where(s => s.StudentId == user.Id)
                .ToListAsync();

            return View(selectedClasses);
        }


        [HttpPost]
        public async Task<IActionResult> SelectedClasses(int classId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var selectedClass = new SelectedClass
            {
                ClassId = classId,
                StudentId = user.Id
            };

            _context.SelectedClasses.Add(selectedClass);
            await _context.SaveChangesAsync();

            //return RedirectToAction("SelectedClasses");
            return Ok(new { message = "Class selected successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> PayForClass(int selectedClassId)
        {
            var selectedClass = await _context.SelectedClasses
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == selectedClassId);

            if (selectedClass == null) return NotFound();

            selectedClass.PaymentStatus = "Paid";

            var enrolledClass = new EnrolledClass
            {
                ClassId = selectedClass.ClassId,
                StudentId = selectedClass.StudentId
            };

            _context.EnrolledClasses.Add(enrolledClass);
            _context.SelectedClasses.Remove(selectedClass); // Remove from selected once paid
            await _context.SaveChangesAsync();

            //return RedirectToAction("Dashboard");
            return RedirectToAction("EnrolledClasses");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelectedClass(int selectedClassId)
        {
            var selectedClass = await _context.SelectedClasses.FindAsync(selectedClassId);
            if (selectedClass != null)
            {
                _context.SelectedClasses.Remove(selectedClass);
                await _context.SaveChangesAsync();
            }
            //return RedirectToAction("Dashboard");
            return RedirectToAction("SelectedClasses");            
        }

    }
}
