using ContactsAPI.Entities;
using ContactsAPI.Models;
using ContactsAPI.Models.User;
using ContactsAPI.Pagination;
using ContactsAPI.Service;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace ContactsAPI.Data.Repositories
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMailService _emailService;
        private readonly IConfiguration _config;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMailService emailService, IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _config = config;
        }

        public Task<MessageHelper<UserDTO>> GetById(string id, string currentUserId)
        {
            throw new NotImplementedException();
        }

        public async Task<MessageHelper> ResetPassword(ResetPasswordDTO reset)
        {
            MessageHelper result = new();

            try
            {
                var user = await _userManager.FindByIdAsync(reset.UserId);

                if (user == null)
                {
                    result.Success = false;
                    result.Message = "Não encontramos este utilizador";
                    return result;
                }

                var resetPassword = await _userManager.ResetPasswordAsync(user, reset.Token, reset.Password);

                if (!resetPassword.Succeeded)
                {
                    string errors = string.Empty;
                    foreach (var error in resetPassword.Errors)
                    {
                        errors += error.Description + "\r";
                    }

                    result.Success = false;
                    result.Message = "Ocorreu um erro ao redefinir a password";
                    return result;
                }
                result.Success = true;
                result.Message = "Password redefinida com sucesso";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ocorreu um erro inesperado ao redefinir a password";

            }
            return result;
        }

        public async Task<MessageHelper> SendEmailForgotPassword(ForgotPasswordDTO dto)
        {
            MessageHelper result = new();

            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);

                if (user == null)
                {
                    result.Success = false;
                    result.Message = "Este email nao existe";
                    return result;
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                string frontendUrl = _config["FrontendUrl"];
                token = HttpUtility.UrlEncode(token);
                string url = $"{frontendUrl}/ResetPassword?userId={user.Id}&token={token}";

                MailRequest mail = new()
                {
                    ToEmail = dto.Email,
                    Subject = "Forgot Password",
                    Body = "<h1>Forgot Password</h1>" + $"<a href='{url}'>Recover</a>",
                    Attachments = null
                };

                await _emailService.SendEmailAsync(mail);

                result.Success = true;
                result.Message = "Email enviado com sucesso";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Não foi possivel enviar o email";
                return result;
            }

            return result;
        }

        public Task<MessageHelper> UpdatePassword(string id, PasswordChangeDTO passwordChange, string currentUserId)
        {
            throw new NotImplementedException();
        }
    }
}
