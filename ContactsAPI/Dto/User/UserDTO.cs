using System.ComponentModel.DataAnnotations;

namespace ContactsAPI.Dto.User
{
    public class UserDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
