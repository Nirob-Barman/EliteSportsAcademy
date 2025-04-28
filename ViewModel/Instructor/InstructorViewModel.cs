namespace EliteSportsAcademy.ViewModel.Instructor
{
    public class InstructorViewModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhotoURL { get; set; }
        public List<string>? ClassesTaken { get; set; }
    }
}
