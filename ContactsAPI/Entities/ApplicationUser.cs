using Microsoft.AspNetCore.Identity;

namespace ContactsAPI.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
