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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public InstructorController(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
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
                    InstructorId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    //InstructorName = model.InstructorName,
                    //InstructorEmail = model.InstructorEmail,
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
            //var claims = User.Claims.ToList();
            //foreach (var claim in claims)
            //{
            //    Console.WriteLine($"{claim.Type}: {claim.Value}");
            //}
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(instructorId))
            {
                return RedirectToAction("Dashboard");
            }
            // Find the current instructor's email
            //var instructorEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            //if (string.IsNullOrEmpty(instructorEmail))
            //{
            //    return RedirectToAction("Dashboard");
            //}

            var instructor = _context.Users.Where(u => u.Id == instructorId).FirstOrDefault();

            // Fetch classes where InstructorEmail matches
            var myClasses = _context.Classes
                .Where(c => c.InstructorId == instructorId)
                //.Where(c => c.InstructorEmail == instructorEmail)
                .Select(c => new ClassViewModel
                {
                    ClassName = c.ClassName,
                    ClassImage = c.ClassImage,
                    //InstructorName = c.InstructorName,
                    //InstructorEmail = c.InstructorEmail,
                    InstructorName = instructor!.UserName,
                    InstructorEmail = instructor.Email,
                    AvailableSeats = c.AvailableSeats,
                    Price = c.Price,
                    Status = c.Status,
                    Feedback = c.Feedback
                })
                .ToList();

            return View(myClasses);
        }


        // GET: Instructor Profile
        public async Task<IActionResult> Profile()
        {
            // Get the current instructor's ID from the claim
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(instructorId))
            {
                return RedirectToAction("Dashboard");
            }

            // Fetch the instructor details from the database using the User ID
            var instructor = await _userManager.FindByIdAsync(instructorId);

            if (instructor == null)
            {
                return RedirectToAction("Dashboard");
            }

            // Map the instructor details to a ProfileViewModel
            var model = new InstructorProfileViewModel
            {
                UserName = instructor.UserName,
                Email = instructor.Email,
                PhotoURL = instructor.PhotoURL,
                FirstName = instructor.FirstName,
                LastName = instructor.LastName
            };

            return View(model);
        }

        // POST: Update Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(InstructorProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the current instructor's ID from the claim
                var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(instructorId))
                {
                    return RedirectToAction("Dashboard");
                }

                // Fetch the instructor details from the database using the User ID
                var instructor = await _userManager.FindByIdAsync(instructorId);

                if (instructor == null)
                {
                    return RedirectToAction("Dashboard");
                }

                // Update the instructor details
                instructor.UserName = model.UserName;
                instructor.Email = model.Email;
                instructor.PhotoURL = model.PhotoURL;
                instructor.FirstName = model.FirstName;
                instructor.LastName = model.LastName;

                // Save the changes to the database
                var result = await _userManager.UpdateAsync(instructor);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Profile updated successfully!";
                    return RedirectToAction(nameof(Profile));
                }

                // If update fails, add the errors to ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            // If validation fails, redisplay the form
            return View("Profile", model);
        }



    }
}
