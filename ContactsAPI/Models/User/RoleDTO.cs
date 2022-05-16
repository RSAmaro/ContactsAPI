using ContactsAPI.Entities;

namespace ContactsAPI.Models.User
{
    public class RoleDTO
    {
        public string Id { get; init; }
        public string Name { get; init; }

        public RoleDTO() { }

        public RoleDTO(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public RoleDTO(ApplicationRole role)
        {
            this.Id = role.Id;
            this.Name = role.Name;
        }
    }
}
