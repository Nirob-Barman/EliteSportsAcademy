using System.ComponentModel.DataAnnotations;

namespace EliteSportsAcademy.ViewModel.Instructor
{
    public class ClassViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Class name is required")]
        public string? ClassName { get; set; }

        [Required(ErrorMessage = "Class image URL is required")]
        public string? ClassImage { get; set; }

        [Required(ErrorMessage = "Instructor name is required")]
        public string? InstructorName { get; set; }

        [Required(ErrorMessage = "Instructor email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? InstructorEmail { get; set; }

        [Required(ErrorMessage = "Available seats are required")]
        [Range(1, int.MaxValue, ErrorMessage = "Available seats must be greater than 0")]
        public int AvailableSeats { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        public string? Status { get; set; }
        public string? Feedback { get; set; }
        public bool IsSelected { get; set; }
    }
}
