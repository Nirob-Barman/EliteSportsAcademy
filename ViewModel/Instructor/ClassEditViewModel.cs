using System.ComponentModel.DataAnnotations;

namespace EliteSportsAcademy.ViewModel.Instructor
{
    public class ClassEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class name is required")]
        public string? ClassName { get; set; }

        public IFormFile? ClassImageFile { get; set; }
        public string? ClassImagePath { get; set; }

        [Required(ErrorMessage = "Available seats are required")]
        [Range(1, int.MaxValue, ErrorMessage = "Available seats must be greater than 0")]
        public int AvailableSeats { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
    }
}
