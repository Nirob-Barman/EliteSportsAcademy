using EliteSportsAcademy.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace EliteSportsAcademy.Models.Instructor
{
    public class Class
    {
        public int Id { get; set; }
        public string? ClassName { get; set; }
        public string? ClassImage { get; set; }
        //public string? InstructorName { get; set; }
        //public string? InstructorEmail { get; set; }
        [Required]
        public string? InstructorId { get; set; }
        public virtual ApplicationUser? Instructor { get; set; }
        public int AvailableSeats { get; set; }
        public decimal Price { get; set; }
        public string? Status { get; set; } = "pending";
        public string? Feedback { get; set; }
    }
}
