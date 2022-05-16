using ContactsAPI.Controllers;
using ContactsAPI.Entities;
using ContactsAPI.Models;
using ContactsAPI.Models.Auth;
using ContactsAPI.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ContactsAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IMailService _mailService;

        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<IdentityOptions> _optionsAccessor;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config,
            IPasswordHasher<ApplicationUser> passwordHasher, IHttpContextAccessor httpContextAccessor,
            IOptions<IdentityOptions> optionsAccessor, IMailService mailService)
        {
            _userManager = userManager;
            _mailService = mailService;
            _config = config;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;
            _optionsAccessor = optionsAccessor;
        }


        public async Task<MessageHelper<AuthDTO>> Login(LoginDTO login)
        {
            MessageHelper<AuthDTO> response = new();
            try
            {
                var user = await AuthenticateUser(login);

                var userRoles = (await _userManager.GetRolesAsync(user)).ToList();
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Incorrect Login.";
                    return response;
                }

                var tokenString = await GenerateJSONWebToken(user, userRoles);

                AuthDTO responseObj = new()
                {
                    Id = user.Id,
                    Token = tokenString,
                    UserName = user.Name,
                    Roles = userRoles.ToArray(),

                };

                SaveToken(responseObj);
                response.Success = true;
                response.obj = responseObj;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }



        private async Task<ApplicationUser> AuthenticateUser(LoginDTO login)
        {
            ApplicationUser? user = null;
            try
            {
                var userfound = await _userManager.FindByEmailAsync(login.Email);
                if (userfound == null)
                {
                    return user;
                }

                if (_passwordHasher.VerifyHashedPassword(userfound, userfound.PasswordHash, login.Password) == PasswordVerificationResult.Success)
                {
                    user = userfound;

                    user.AccessFailedCount = 0;
                    user.LockoutEnd = null;

                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    await UserAccessFailed(userfound);
                }
                return user;

            }
            catch (Exception)
            {
                return user;
            }

        }

        private async Task UserAccessFailed(ApplicationUser user)
        {

            user.AccessFailedCount++;
            await _userManager.UpdateAsync(user);
        }

        private async Task<string> GenerateJSONWebToken(ApplicationUser userInfo, List<string> userRoles)
        {
            var user = await _userManager.FindByIdAsync(userInfo.Id);

            var userClaims = await _userManager.GetClaimsAsync(user);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = GetTokenClaims(user).Union(userClaims).ToList();

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"], claims: claims, signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static IEnumerable<Claim> GetTokenClaims(IdentityUser user)
        {
            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id)
            };
        }

        public virtual MessageHelper<bool> SaveToken(AuthDTO user)
        {
            MessageHelper<bool> response = new();

            try
            {
                _httpContextAccessor.HttpContext.Session.SetString("token", user.Token);
                _httpContextAccessor.HttpContext.Session.SetString("username", user.UserName);
                _httpContextAccessor.HttpContext.Session.SetString("id", user.Id);
                _httpContextAccessor.HttpContext.Session.SetString("roles", String.Join(",", user.Roles));

                response.Success = true;
                response.obj = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.obj = false;
                response.Message = ex.Message;

            }

            return response;
        }

        public async Task<MessageHelper> ConfirmEmail(ConfirmEmailDTO dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return new MessageHelper
                {
                    Success = false,
                    Message = "User not Found!"
                };

            var decodedToken = WebEncoders.Base64UrlDecode(dto.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new MessageHelper
                {
                    Success = true,
                    Message = "Email Confirmed Successfuly!",
                };

            return new MessageHelper
            {
                Success = false,
                Message = result.Errors.FirstOrDefault()!.Description,
            };
        }

        public async Task<MessageHelper> ForgotPassword(string email)
        {
            try
            {
                MessageHelper result = new();
                var account = await _userManager.FindByEmailAsync(email);
                if (account == null) return new MessageHelper() { Success = false, Message = "No account found with that email!" };

                var token = await _userManager.GeneratePasswordResetTokenAsync(account);
                string url = $"{_config["AppUrl"]}/api/auth/ResetPassword?token={token}&email={email}";

                MailRequest mail = new()
                {
                    ToEmail = email,
                    Subject = "Forgot Password",
                    Body = "<h1>Forgot Password</h1>" + $"<a href='{url}'>Recover</a>",
                    Attachments = null
                };

                await _mailService.SendEmailAsync(mail);

                result.Success = true;
                result.Message = "Forgot Password request complete!";
                return result;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
