namespace ContactsAPI.Models.User
{
    public class ResetPasswordDTO
    {
        public string Password { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
    }
}
