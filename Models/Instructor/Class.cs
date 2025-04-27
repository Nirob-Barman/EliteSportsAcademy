namespace EliteSportsAcademy.Models.Instructor
{
    public class Class
    {
        public int Id { get; set; }
        public string? ClassName { get; set; }
        public string? ClassImage { get; set; }
        public string? InstructorName { get; set; }
        public string? InstructorEmail { get; set; }
        public int AvailableSeats { get; set; }
        public decimal Price { get; set; }
    }
}
