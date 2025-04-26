using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EliteSportsAcademy.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        // User-specific dashboard
        [Authorize(Roles = "User")]
        public IActionResult UserDashboard()
        {
            return View();
        }

        // Instructor-specific dashboard
        [Authorize(Roles = "Instructor")]
        public IActionResult InstructorDashboard()
        {
            return View();
        }

        // Admin-specific dashboard
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }
    }
}
