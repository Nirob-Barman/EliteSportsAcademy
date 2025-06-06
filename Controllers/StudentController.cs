﻿using System.Security.Claims;
using EliteSportsAcademy.Data;
using EliteSportsAcademy.Models.Account;
using EliteSportsAcademy.Models.Student;
using EliteSportsAcademy.ViewModel.Student;
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
        public async Task<IActionResult> Dashboard()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }
            var student = await _userManager.FindByEmailAsync(email);
            if (student == null)
            {
                return NotFound("Student not found");
            }


            var selectedCount = await _context.SelectedClasses.CountAsync(s => s.StudentId == student.Id && s.PaymentStatus == "Pending");
            var enrolledCount = await _context.EnrolledClasses.CountAsync(e => e.StudentId == student.Id);
            var totalAvailableClasses = await _context.Classes.CountAsync(c => c.AvailableSeats > 0 && c.Status == "approved");
            var dashboardVM = new StudentDashboardViewModel
            {
                SelectedCount = selectedCount,
                EnrolledCount = enrolledCount,
                TotalAvailableClasses = totalAvailableClasses
            };

            return View(dashboardVM);
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

            var classEntity = await _context.Classes.FindAsync(classId);
            if (classEntity == null)
                return NotFound();

            if (classEntity.AvailableSeats <= 0)
                return BadRequest(new { message = "No available seats." });

            bool alreadySelected = await _context.SelectedClasses.AnyAsync(s => s.StudentId == user.Id && s.ClassId == classId);
            if (alreadySelected)
                return BadRequest(new { message = "You have already selected this class." });

            var selectedClass = new SelectedClass
            {
                ClassId = classId,
                StudentId = user.Id
            };

            _context.SelectedClasses.Add(selectedClass);

            // Reduce available seats
            classEntity.AvailableSeats -= 1;
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
            //var selectedClass = await _context.SelectedClasses.FindAsync(selectedClassId);
            var selectedClass = await _context.SelectedClasses
                .Include(sc => sc.Class) // Ensure we load the related class
                .FirstOrDefaultAsync(sc => sc.Id == selectedClassId);
            if (selectedClass != null)
            {
                selectedClass.Class!.AvailableSeats += 1;
                _context.SelectedClasses.Remove(selectedClass);
                await _context.SaveChangesAsync();
            }
            //return RedirectToAction("Dashboard");
            return RedirectToAction("SelectedClasses");            
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (studentId == null) return RedirectToAction("Login", "Account");

            var student = await _userManager.FindByIdAsync(studentId);
            if (student == null) return RedirectToAction("Login", "Account");

            var model = new StudentProfileViewModel
            {
                UserName = student.UserName,
                Email = student.Email,
                PhotoURL = student.PhotoURL,
                FirstName = student.FirstName,
                LastName = student.LastName
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(StudentProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (studentId == null) return RedirectToAction("Login", "Account");

            var student = await _userManager.FindByIdAsync(studentId);
            if (student == null) return RedirectToAction("Login", "Account");

            if (model.Photo != null && model.Photo.Length > 0)
            {
                var extension = Path.GetExtension(model.Photo.FileName);
                // Optionally: validate extension
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                if (!allowedExtensions.Contains(extension.ToLower()))
                {
                    ModelState.AddModelError("Photo", "Only image files (.jpg, .png, .gif) are allowed.");
                    return View("Profile", model);
                }

                // Delete the old photo if it exists
                if (!string.IsNullOrEmpty(student.PhotoURL))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", student.PhotoURL.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Create unique file name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Photo.FileName);
                //var fileName = Guid.NewGuid().ToString() + extension;
                // Define upload folder
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Account");

                // Ensure folder exists
                //if (!Directory.Exists(uploadsFolder))
                //    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Photo.CopyToAsync(fileStream);
                }

                // Update PhotoURL (accessible via browser)
                student.PhotoURL = "/uploads/Account/" + fileName;
            }

            student.UserName = model.UserName;
            student.Email = model.Email;
            //student.PhotoURL = model.PhotoURL;
            student.FirstName = model.FirstName;
            student.LastName = model.LastName;

            var result = await _userManager.UpdateAsync(student);

            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Profile", model);
        }

    }
}
