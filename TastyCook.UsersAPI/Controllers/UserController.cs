using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using TastyCook.UsersAPI.Entities;
using TastyCook.UsersAPI.Models;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.UsersAPI.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;


        public UserController(UserManager<User> userManager,
            IConfiguration configuration,
            IPublishEndpoint publishEndpoint,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public class AuthenticateRequest
        {
            [Required] public string IdToken { get; set; }
        }
        

        [HttpGet]
        [Route("test")]
        [Authorize]
        public string Test()
        {
            return "Hello";
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("googleAuthenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest data)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start authentication with Google account {data.IdToken}");
                GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();

                // Change this to your google client ID
                settings.Audience = new List<string>()
                    { "949493455412-lab5qmkch79a6ilg7u9vrin4lbhe024q.apps.googleusercontent.com" };

                GoogleJsonWebSignature.Payload
                    payload = GoogleJsonWebSignature.ValidateAsync(data.IdToken, settings).Result;
                var token = await CreateTokenAsync(new UserModel() { Email = payload.Email });
                _logger.LogInformation($"{DateTime.Now} | Successful authentication with Google account {data.IdToken}");

                return Ok(new { AuthToken = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        [Route("register")]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Register([FromBody] UserModel user)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start creating new user, email {user.Email}");

                User identityUser = new User { Email = user.Email, UserName = user.Username };
                var result = await _userManager.CreateAsync(identityUser, user.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"{DateTime.Now} | User successfully created, email {user.Email}");
                    var userFromDb = await _userManager.FindByEmailAsync(user.Email);

                    _logger.LogInformation($"{DateTime.Now} | Start sending new user to message broker, id {userFromDb.Id}");
                    await _publishEndpoint.Publish(new UserItemCreated(userFromDb.Id, user.Email, user.Username, user.Password));
                    _logger.LogInformation($"{DateTime.Now} | End sending new user to message broker, id {userFromDb.Id}");
                }

                return !result.Succeeded ? new BadRequestObjectResult(result) : StatusCode(201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LogIn([FromBody] UserModel model)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start logging in, email {model.Email}");
                var result = !await ValidateUserAsync(model);
                var token = await CreateTokenAsync(model);
                _logger.LogInformation($"{DateTime.Now} | End logging in, email {model.Email}");

                return result ? Unauthorized() : Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> LogOut()
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start changing password, email {User.Identity.Name}");

                if (changePasswordModel.NewPassword == changePasswordModel.CurrentPassword)
                {
                    _logger.LogInformation($"{DateTime.Now} | End changing passwords: same passwords, email {User.Identity.Name}");
                    return Ok("Entered password is the same");
                }

                if (changePasswordModel.NewPassword != changePasswordModel.RepeatNewPassword)
                {
                    _logger.LogInformation($"{DateTime.Now} | End changing passwords: different passwords, email {User.Identity.Name}");
                    return BadRequest("Passwords must be the same.");
                }

                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                if (user == null)
                {
                    return NotFound();
                }

                var result = await _userManager.ChangePasswordAsync(user, changePasswordModel.CurrentPassword,
                    changePasswordModel.NewPassword);

                _logger.LogInformation($"{DateTime.Now} | End changing passwords, email {User.Identity.Name}");
                if (result.Succeeded)
                {
                    _logger.LogInformation($"{DateTime.Now} | Start sending updated user to message broker, email {User.Identity.Name}");
                    await _publishEndpoint.Publish(new UserItemUpdated(user.Id, user.Email, user.UserName, changePasswordModel.NewPassword));
                    _logger.LogInformation($"{DateTime.Now} | End sending updated user to message broker, email {User.Identity.Name}");

                    return Ok();
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("changeEmail")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailModel changeEmailModel)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start changing email, oldEmail {User.Identity.Name}");

                string userEmail = User.Identity.Name;
                var user = await _userManager.FindByEmailAsync(userEmail);

                if (changeEmailModel.NewEmail == userEmail)
                {
                    _logger.LogInformation($"{DateTime.Now} | End changing email: same emails, email {userEmail}");
                    return Ok("Entered email is the same");
                }

                user.Email = changeEmailModel.NewEmail;
                var result = await _userManager.UpdateAsync(user);

                _logger.LogInformation($"{DateTime.Now} | End changing email, oldEmail {userEmail}");

                if (result.Succeeded)
                {
                    _logger.LogInformation($"{DateTime.Now} | Start sending updated user to message broker, oldEmail {userEmail}");
                    await _publishEndpoint.Publish(new UserItemUpdated(user.Id, changeEmailModel.NewEmail, user.UserName, null));
                    _logger.LogInformation($"{DateTime.Now} | End sending updated user to message broker, oldEmail {userEmail}");

                    return Ok();
                }

                return StatusCode(500, result.Errors.First());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        private async Task<bool> ValidateUserAsync(UserModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var result = user != null && await _userManager.CheckPasswordAsync(user, model.Password);
            return result;
        }

        private async Task<string> CreateTokenAsync(UserModel model)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(model);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var jwtConfig = _configuration.GetSection("JwtConfig");
            var key = Encoding.UTF8.GetBytes(jwtConfig["Secret"]);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(UserModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email)
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtConfig");
            var tokenOptions = new JwtSecurityToken
            (
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresIn"])),
                signingCredentials: signingCredentials
            );
            return tokenOptions;
        }
    }
}