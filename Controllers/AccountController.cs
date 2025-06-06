﻿using EliteSportsAcademy.Models.Account;
using EliteSportsAcademy.ViewModel.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EliteSportsAcademy.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string photoFileName = null!;
                if (model.Photo != null && model.Photo.Length > 0)
                {
                    var extension = Path.GetExtension(model.Photo.FileName);
                    // Optionally: validate extension
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    if (!allowedExtensions.Contains(extension.ToLower()))
                    {
                        ModelState.AddModelError("Photo", "Only image files (.jpg, .png, .gif) are allowed.");
                        return View("Register", model);
                    }

                    // Create a unique file name
                    photoFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Photo.FileName);

                    // Define the path to save the file (e.g., wwwroot/uploads/Account)
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Account");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var filePath = Path.Combine(uploadsFolder, photoFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Photo.CopyToAsync(stream);
                    }
                }

                // Create a new user
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Username,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Address = model.Address,
                    IsAgreedToTerms = model.IsAgreedToTerms,
                    //PhotoURL = model.PhotoURL,
                    PhotoURL = photoFileName != null ? "/uploads/Account/" + photoFileName : null,
                };

                // Add user to the database
                var result = await _userManager.CreateAsync(user, model.Password!);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Student");
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect to the home page or dashboard
                    return RedirectToAction("Index", "Home");
                }

                // If there are errors, add them to ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed; redisplay the form
            return View(model);
        }


        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl; // Store the return URL for redirection after login
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email!);

                if (user != null)
                {
                    var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password!);
                    if (isPasswordValid)
                    {
                        // Sign in the user
                        var result = await _signInManager.PasswordSignInAsync(
                            user!,
                            model.Password!,
                            model.RememberMe,
                            lockoutOnFailure: false);

                        if (result.Succeeded)
                        {
                            //var userPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
                            //var identity = (ClaimsIdentity)userPrincipal.Identity!;
                            //// Check if email claim already exists
                            //if (!identity.HasClaim(c => c.Type == ClaimTypes.Email))
                            //{
                            //    identity.AddClaim(new Claim(ClaimTypes.Email, user.Email ?? ""));
                            //}

                            //await _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme, userPrincipal);
                            // Redirect to the home page or the intended page
                            return RedirectToAction("Index", "Home");
                        }
                        else if (result.IsLockedOut)
                        {
                            ModelState.AddModelError(string.Empty, "Your account is locked out. Please try again later.");
                        }
                        else if (result.IsNotAllowed)
                        {
                            ModelState.AddModelError(string.Empty, "You are not allowed to log in. Please confirm your email.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.Password), "Incorrect password.");
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(model.Email), "No account found with this email.");
                }
            }
            // If we got this far, something failed; redisplay the form
            return View(model);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
