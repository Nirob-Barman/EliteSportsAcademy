using EliteSportsAcademy.Data;
using Microsoft.AspNetCore.Mvc;

namespace EliteSportsAcademy.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;
        public StudentController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
