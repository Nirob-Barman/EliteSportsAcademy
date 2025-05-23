﻿using System.Security.Claims;
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


        [AllowAnonymous]
        public IActionResult InstructorClasses(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return NotFound();
            }

            var instructor = _context.Users.FirstOrDefault(u => u.Email == email);
            if (instructor == null)
            {
                return NotFound();
            }

            var approvedClasses = _context.Classes
                .Where(c => c.InstructorId == instructor.Id && c.Status == "Approved")
                .Select(c => new ClassViewModel
                {
                    ClassName = c.ClassName,
                    ClassImagePath = c.ClassImage,
                    InstructorName = instructor.UserName,
                    InstructorEmail = instructor.Email,
                    AvailableSeats = c.AvailableSeats,
                    Price = c.Price,
                    Status = c.Status,
                    Feedback = c.Feedback
                })
                .ToList();

            ViewBag.InstructorName = instructor.UserName;

            return View(approvedClasses);
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
            if (model.ClassImageFile == null || model.ClassImageFile.Length == 0)
            {
                ModelState.AddModelError("ClassImageFile", "Please upload a class image.");
            }
            if (ModelState.IsValid)
            {
                string? imagePath = null;
                if (model.ClassImageFile != null && model.ClassImageFile.Length > 0)
                {
                    var extension = Path.GetExtension(model.ClassImageFile.FileName).ToLower();
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("ClassImageFile", "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                        return View(model);
                    }

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Instructor/ClassImages");
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ClassImageFile.CopyToAsync(stream);
                    }

                    imagePath = "/uploads/Instructor/ClassImages/" + fileName;
                }

                // Map ViewModel to Entity (if needed)
                var newClass = new Class
                {
                    ClassName = model.ClassName,
                    //ClassImage = model.ClassImagePath,
                    ClassImage = imagePath,
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
            else
            {
                var allErrors = ModelState
                    .Where(m => m.Value!.Errors.Count > 0)
                    .Select(m => new {
                        Field = m.Key,
                        Errors = m.Value!.Errors.Select(e => e.ErrorMessage)
                    });
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
                    Id = c.Id,
                    ClassName = c.ClassName,
                    ClassImagePath = c.ClassImage,
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

        [HttpGet]
        public IActionResult EditClass(int id)
        {
            var classItem = _context.Classes.FirstOrDefault(c => c.Id == id);
            if (classItem == null) return NotFound();

            var viewModel = new ClassEditViewModel
            {
                Id = classItem.Id,
                ClassName = classItem.ClassName,
                ClassImagePath = classItem.ClassImage,
                AvailableSeats = classItem.AvailableSeats,
                Price = classItem.Price,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditClass(ClassEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var classItem = _context.Classes.FirstOrDefault(c => c.Id == model.Id);
            if (classItem == null) return NotFound();

            classItem.ClassName = model.ClassName;
            classItem.AvailableSeats = model.AvailableSeats;
            classItem.Price = model.Price;


            // Handle image upload
            if (model.ClassImageFile != null)
            {
                var extension = Path.GetExtension(model.ClassImageFile.FileName).ToLower();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ClassImageFile", "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                    return View(model);
                }

                // Delete the old image file if it exists
                if (!string.IsNullOrEmpty(classItem.ClassImage))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", classItem.ClassImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath); // Remove the old image
                    }
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ClassImageFile.FileName);
                //var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Instructor/ClassImages");
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Instructor", "ClassImages");
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ClassImageFile.CopyToAsync(stream);
                }

                // Save relative path to database
                classItem.ClassImage = "/uploads/Instructor/ClassImages/" + fileName;
            }

            _context.SaveChanges();

            return RedirectToAction("MyClasses");
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
                
                if (model.Photo != null && model.Photo.Length > 0)
                {
                    var extension = Path.GetExtension(model.Photo!.FileName);
                    // Optionally: validate extension
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    if (!allowedExtensions.Contains(extension.ToLower()))
                    {
                        ModelState.AddModelError("Photo", "Only image files (.jpg, .png, .gif) are allowed.");
                        return View("Profile", model);
                    }

                    if (!string.IsNullOrEmpty(instructor.PhotoURL))
                    {
                        // Get the full path to the old photo
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", instructor.PhotoURL.TrimStart('/'));

                        // Check if the file exists and delete it
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);  // Delete the old photo file
                        }
                    }

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Account");

                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Photo.CopyToAsync(stream);
                    }

                    instructor.PhotoURL = "/uploads/Account/" + fileName;
                }

                // Update the instructor details
                instructor.UserName = model.UserName;
                instructor.Email = model.Email;
                //instructor.PhotoURL = model.PhotoURL;
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
