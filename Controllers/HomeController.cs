using System.Diagnostics;
using EliteSportsAcademy.Models;
using EliteSportsAcademy.Models.Account;
using EliteSportsAcademy.ViewModel.Instructor;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EliteSportsAcademy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Instructors()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<InstructorViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Contains("Instructor"))
                {
                    userList.Add(new InstructorViewModel
                    {
                        Name = user.UserName,
                        Email = user.Email,
                        PhotoURL = user.PhotoURL
                    });
                }
            }
            return View(userList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            // Pass exception details in Development environment
            if (HttpContext.Request.Host.Value.Contains("localhost"))
            {
                var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionHandlerPathFeature?.Error != null)
                {
                    ViewBag.ExceptionType = exceptionHandlerPathFeature.Error.GetType().Name;
                    ViewBag.ExceptionMessage = exceptionHandlerPathFeature.Error.Message;
                    ViewBag.ExceptionStackTrace = exceptionHandlerPathFeature.Error.StackTrace;
                }
            }

            return View(errorViewModel);
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult NotFound()
        {
            //_logger.LogWarning("404 Not Found: {Path}", HttpContext.Request.Path);
            return View(); // Returns the NotFound.cshtml view
        }
    }
}
