using Microsoft.AspNetCore.Identity;

namespace ReactBackend.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}