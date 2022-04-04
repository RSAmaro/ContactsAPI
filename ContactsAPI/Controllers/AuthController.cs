using ContactsAPI.Entities;
using ContactsAPI.Models;
using ContactsAPI.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            _authService = authService;
            _userManager = userManager;
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
                    EmailConfirmed = true,
                    UserName = authDTO.Email,
                    Name = authDTO.Username!,
                };

                var resultCreateUserInDatabase = await _userManager.CreateAsync(userInDatabase, authDTO.Password);
                await _userManager.AddToRoleAsync(userInDatabase, Roles.Member.Value );
                if (!resultCreateUserInDatabase.Succeeded)
                {
                    result.Success = false;
                    result.Message = "Error creating an user!";
                    return Ok(result);
                }

                result.Success = true;
                result.Message = "User Created!";
            }
            catch (Exception)
            {
                result.Success = false;
                result.Message = "Error creating an user.";
            }

            return Ok(result);
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
