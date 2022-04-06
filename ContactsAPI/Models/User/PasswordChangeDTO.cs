namespace ContactsAPI.Models.User
{
    public class PasswordChangeDTO
    {
        public string Id { get; set; }
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
