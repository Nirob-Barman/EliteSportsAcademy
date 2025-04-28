using EliteSportsAcademy.Data;
using EliteSportsAcademy.ViewModel.Instructor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EliteSportsAcademy.Controllers
{
    public class ClassController : Controller
    {
        private readonly AppDbContext _context;

        public ClassController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var classes = await _context.Classes
                .Where(c => c.Status == "approved") // show only approved classes
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
                .ToListAsync();

            return View(classes);
        }
    }
}
