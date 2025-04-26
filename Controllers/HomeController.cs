using System.Diagnostics;
using EliteSportsAcademy.Models;
using Microsoft.AspNetCore.Mvc;

namespace EliteSportsAcademy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //var a = 3;
            //var b = 0;
            //var c = a / b;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult NotFound()
        {
            _logger.LogWarning("404 Not Found: {Path}", HttpContext.Request.Path);
            return View(); // Returns the NotFound.cshtml view
        }
    }
}
