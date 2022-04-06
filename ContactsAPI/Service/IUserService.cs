using ContactsAPI.Models;
using ContactsAPI.Models.User;

namespace ContactsAPI.Data.Repositories
{
    public interface IUserService
    {
        Task<MessageHelper> ResetPassword(ResetPasswordDTO reset);
        Task<MessageHelper> SendEmailForgotPassword(ForgotPasswordDTO dto);
    }
}