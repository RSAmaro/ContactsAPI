using Ardalis.SmartEnum;
using Microsoft.AspNetCore.Identity;

namespace ContactsAPI.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        {

        }

        public ApplicationRole(string roleName) : base(roleName)
        {

        }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }

    }

    public sealed class Roles: SmartEnum<Roles, string>
    {
        public static readonly Roles Admin = new("Admin", "Admin");
        public static readonly Roles Member = new("Member", "Member");

        protected Roles(string name, string value): base(name, value) 
        {

        }
    }
}