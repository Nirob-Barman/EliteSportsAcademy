using System.ComponentModel.DataAnnotations;

namespace EliteSportsAcademy.ViewModel.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\W).*$", ErrorMessage = "Password must contain at least one capital letter and one special character")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

        public string? PhotoURL { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }
        [Required(ErrorMessage = "You must agree to the terms and conditions.")]
        [Display(Name = "I agree to the terms and conditions")]
        public bool IsAgreedToTerms { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
