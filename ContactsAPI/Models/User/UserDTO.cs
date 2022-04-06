using ContactsAPI.Entities;

namespace ContactsAPI.Models.User
{
    public class UserDTO
    {
        private string id { get; set; }
        private string name { get; set; }

        private string email { get; set; }

        private List<RoleDTO> role { get; set; }

        public string Id { get => id; }
        public string Name { get => name; }

        public string Email { get => email; }

        public List<RoleDTO> Role { get => role; }

        public UserDTO(ApplicationUser user)
        {
            this.id = user.Id;
            this.name = user.Name;
            this.email = user.Email;
            this.role = user.UserRoles.Select(r => new RoleDTO(r.Role)).ToList();
        }
    }
}
