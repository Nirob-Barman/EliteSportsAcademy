using Microsoft.AspNetCore.Identity;

namespace EliteSportsAcademy.Models.Account
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhotoURL { get; set; }
        public bool IsAgree { get; set; }
    }
}
