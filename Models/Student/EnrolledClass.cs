using EliteSportsAcademy.Models.Account;
using EliteSportsAcademy.Models.Instructor;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EliteSportsAcademy.Models.Student
{
    public class EnrolledClass
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        public Class? Class { get; set; }

        [Required]
        public string? StudentId { get; set; }

        [ForeignKey("StudentId")]
        public ApplicationUser? Student { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.Now;
    }
}
