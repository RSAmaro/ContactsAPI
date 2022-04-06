using ContactsAPI.Data.Repositories;
using ContactsAPI.Models;
using ContactsAPI.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _users;
        public UserController(IUserService users)
        {
            _users = users;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public async Task<MessageHelper> ForgotPassword(ForgotPasswordDTO dto)
        {
            return await _users.SendEmailForgotPassword(dto);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public async Task<MessageHelper> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            return await _users.ResetPassword(resetPasswordDTO);
        }


    }
}
