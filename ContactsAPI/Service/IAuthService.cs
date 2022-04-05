using ContactsAPI.Models;
using ContactsAPI.Models.User;

namespace ContactsAPI.Controllers
{
    public interface IAuthService
    {
        Task<MessageHelper<AuthDTO>> Login(LoginDTO body);

        Task<MessageHelper> ConfirmEmail(string userId, string token);
    }
}