using ContactsAPI.Entities;
using ContactsAPI.Models;
using ContactsAPI.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using ContactsAPI.Service;

namespace ContactsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;

        public AuthController(UserManager<ApplicationUser> userManager, IAuthService authService, IConfiguration configuration, IMailService mailService)
        {
            _authService = authService;
            _userManager = userManager;
            _mailService = mailService;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<MessageHelper<AuthDTO>> Login([FromBody] LoginDTO body)
        {
            return await _authService.Login(body);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register(RegisterDTO authDTO)
        {
            MessageHelper result = new();
            try
            {
                RegisterDTOValidator validator = new();
                var responseValidatorDTO = await validator.ValidateAsync(authDTO);
                if (!responseValidatorDTO.IsValid)
                {
                    result.Success = false;
                    result.Message = responseValidatorDTO.Errors.FirstOrDefault()!.ErrorMessage;
                    return Ok(result);
                }

                var passwordValidator = new PasswordValidator<ApplicationUser>();
                var resultValidatePassword = await passwordValidator.ValidateAsync(_userManager, null!, authDTO.Password);
                if (!resultValidatePassword.Succeeded)
                {
                    result.Success = false;
                    result.Message = "Incorrect Password Format (At least one Uppercase, symbol and number)";
                    return Ok(result);
                }

                ApplicationUser userInDatabase = await _userManager.FindByEmailAsync(authDTO.Email);
                if (userInDatabase != null)
                {
                    result.Success = false;
                    result.Message = "There's already an account with that email!";
                    return Ok(result);
                }

                userInDatabase = new ApplicationUser
                {
                    Email = authDTO.Email,
                    EmailConfirmed = false,
                    UserName = authDTO.Email,
                    Name = authDTO.Username!,
                };
                
                 var resultCreateUserInDatabase = await _userManager.CreateAsync(userInDatabase, authDTO.Password);

                 await _userManager.AddToRoleAsync(userInDatabase, Roles.Member.Value );
                 if (!resultCreateUserInDatabase.Succeeded)
                 {
                     result.Success = false;
                     result.Message = "Couldn't assign role to new user!";
                     return Ok(result);
                 }
                

                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(userInDatabase);

                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_configuration["AppUrl"]}/api/auth/ConfirmEmail?userid={userInDatabase.Id}&token={validEmailToken}";
                MailRequest mail = new()
                {
                    ToEmail = userInDatabase.Email,
                    Subject = "Confirm your Email",
                    Body = "<h1>Confirm your Email</h1>" + $"<a href='{url}'>Confirm Email</a>",
                    Attachments = null
                };
                
                await _mailService.SendEmailAsync(mail);

                result.Success = true;
                result.Message = "User Created, please verify email!";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Error creating an user.";
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
                return NotFound();

            var result = await _authService.ConfirmEmail(userId, token);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }



        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public IActionResult Logout()
        {
            MessageHelper<object> response = new();

            try
            {
                HttpContext.Session.Clear();
                response.Success = true;
                response.Message = "";

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.obj = null;
                response.Message = ex.Message;
            }
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetUser()
        {
            MessageHelper<AuthDTO> response = new();

            try
            {
                response.Success = true;
                response.Message = "";
                response.obj = new AuthDTO()
                {
                    Token = HttpContext.Session.GetString("token") ?? null,
                    UserName = HttpContext.Session.GetString("username") ?? null,
                    Id = HttpContext.Session.GetString("id") ?? null,
                    Roles = HttpContext.Session.GetString("roles")?.Split(",") ?? null,
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.obj = null;
            }

            return Ok(response);
        }

    }

    
}
