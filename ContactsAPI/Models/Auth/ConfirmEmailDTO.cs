namespace ContactsAPI.Models.Auth
{
    public class ConfirmEmailDTO
    {
        public string Token { get; set; }
        public string UserId { get; set; }
    }
}
