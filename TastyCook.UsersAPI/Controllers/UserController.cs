using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TastyCook.UsersAPI.Entities;
using TastyCook.UsersAPI.Models;
using static MassTransit.ValidationResultExtensions;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.UsersAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;


        public UserController(UserManager<User> userManager,
            IConfiguration configuration,
            IPublishEndpoint publishEndpoint,
            ILogger<UserController> logger,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
            _roleManager = roleManager;
        }

        public class AuthenticateRequest
        {
            [Required] public string IdToken { get; set; }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("google-authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest data)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start authentication with Google account {data.IdToken}");
                GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();

                settings.Audience = new List<string>() { "831416949571-31eonrv6akqo426nnrrmjar8htnguboa.apps.googleusercontent.com" };

                GoogleJsonWebSignature.Payload payload = GoogleJsonWebSignature.ValidateAsync(data.IdToken, settings).Result;

                var user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    User identityUser = new User { Email = payload.Email, UserName = payload.Name };
                    var result = await _userManager.CreateAsync(identityUser);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"{DateTime.Now} | User successfully created, email {payload.Email}");
                        var userFromDb = await _userManager.FindByEmailAsync(payload.Email);

                        _logger.LogInformation($"{DateTime.Now} | Start sending new user to message broker, id {userFromDb.Id}");
                        var userRole = await _userManager.IsInRoleAsync(userFromDb, "Admin") ? "Admin" : "User";
                        await _publishEndpoint.Publish(new UserItemCreated(userFromDb.Id, payload.Email, payload.Name, userRole));
                        _logger.LogInformation($"{DateTime.Now} | End sending new user to message broker, id {userFromDb.Id}");
                    }
                }
                else if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    return BadRequest("There is already account with this email");
                }

                var token = await CreateTokenAsync(new UserModel() { Email = payload.Email });
                _logger.LogInformation($"{DateTime.Now} | Successful authentication with Google account {data.IdToken}");

                return Ok(new { AuthToken = token, IsExternalLogin = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
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
                    await _publishEndpoint.Publish(new UserItemCreated(userFromDb.Id, user.Email, user.Username, "User"));
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
        [AllowAnonymous]
        public async Task<IActionResult> LogIn([FromBody] UserModel model)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start logging in, email {model.Email}");
                var result = !await ValidateUserAsync(model);

                if (result) return NotFound("No such email or some fields are incorrect");

                var token = await CreateTokenAsync(model);
                _logger.LogInformation($"{DateTime.Now} | End logging in, email {model.Email}");

                return result ? Unauthorized() : Ok(new { Token = token, IsExternalLogin = false });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start changing password, email {User.Identity.Name}");

                if (changePasswordModel.NewPassword == changePasswordModel.CurrentPassword)
                {
                    _logger.LogInformation($"{DateTime.Now} | End changing passwords: same passwords, email {User.Identity.Name}");
                    return BadRequest("Entered password is the same");
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
                    var userRole = await _userManager.IsInRoleAsync(user, "Admin") ? "Admin" : "User";
                    await _publishEndpoint.Publish(new UserItemUpdated(user.Id, user.Email, user.UserName, userRole));
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
        [Route("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailModel changeEmailModel)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start changing email, oldEmail {User.Identity.Name}");

                string userEmail = User.Identity.Name;
                if (changeEmailModel.NewEmail == userEmail)
                {
                    _logger.LogInformation($"{DateTime.Now} | End changing email: same emails, email {userEmail}");
                    return BadRequest("Entered email is the same");
                }

                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user is null)
                {
                    return NotFound();
                }

                user.Email = changeEmailModel.NewEmail;
                var result = await _userManager.UpdateAsync(user);

                _logger.LogInformation($"{DateTime.Now} | End changing email, oldEmail {userEmail}");

                if (result.Succeeded)
                {
                    _logger.LogInformation($"{DateTime.Now} | Start sending updated user to message broker, oldEmail {userEmail}");
                    var userRole = await _userManager.IsInRoleAsync(user, "Admin") ? "Admin" : "User";
                    await _publishEndpoint.Publish(new UserItemUpdated(user.Id, changeEmailModel.NewEmail, user.UserName, userRole));
                    _logger.LogInformation($"{DateTime.Now} | End sending updated user to message broker, oldEmail {userEmail}");
                    var newToken = await CreateTokenAsync(new UserModel() { Email = changeEmailModel.NewEmail });

                    return Ok(new { Token = newToken });
                }

                return StatusCode(500, result.Errors.First());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpGet]
        [Route("current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Get current user");
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                var userResponse = await MapUserToResponse(user);

                return userResponse is null ? new BadRequestObjectResult(userResponse) : Ok(userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start Get all users");
                var users = _userManager.Users.ToList();
                var usersResponse = await MapUsersToResponse(users);
                _logger.LogInformation($"{DateTime.Now} | End Get all users");

                return Ok(usersResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start Delete user, id: {id}");
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"There is no user with this id: {id}");
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"{DateTime.Now} | Start sending updated user to message broker, email {User.Identity.Name}");
                    await _publishEndpoint.Publish(new UserItemDeleted(user.Id));
                    _logger.LogInformation($"{DateTime.Now} | End sending updated user to message broker, email {User.Identity.Name}");
                }

                _logger.LogInformation($"{DateTime.Now} | End Delete user, id: {id}");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("{userId}/change-password")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangePasswordAdmin(string userId, [FromBody] ChangePasswordModel changePasswordModel)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start changing password, id {userId}");

                if (changePasswordModel.NewPassword != changePasswordModel.RepeatNewPassword)
                {
                    _logger.LogInformation($"{DateTime.Now} | End changing passwords: different passwords, id {userId}");
                    return BadRequest("Passwords must be the same.");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }
                
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, changePasswordModel.NewPassword);

                _logger.LogInformation($"{DateTime.Now} | End changing passwords, id {userId}");
                if (result.Succeeded)
                {
                    _logger.LogInformation($"{DateTime.Now} | Start sending updated user to message broker, id {userId}");
                    var userRole = await _userManager.IsInRoleAsync(user, "Admin") ? "Admin" : "User";
                    await _publishEndpoint.Publish(new UserItemUpdated(user.Id, user.Email, user.UserName, userRole));
                    _logger.LogInformation($"{DateTime.Now} | End sending updated user to message broker, id {userId}");

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
        [Route("{userId}/change-email")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeEmailAdmin(string userId, [FromBody] ChangeEmailModel changeEmailModel)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start changing email, id {userId}");

                var user = await _userManager.FindByIdAsync(userId);
                if (user is null)
                {
                    return NotFound();
                }

                user.Email = changeEmailModel.NewEmail;
                var result = await _userManager.UpdateAsync(user);

                _logger.LogInformation($"{DateTime.Now} | End changing email, id {userId}");

                if (result.Succeeded)
                {
                    _logger.LogInformation($"{DateTime.Now} | Start sending updated user to message broker, id {userId}");
                    var userRole = await _userManager.IsInRoleAsync(user, "Admin") ? "Admin" : "User";
                    await _publishEndpoint.Publish(new UserItemUpdated(user.Id, changeEmailModel.NewEmail, user.UserName, userRole));
                    _logger.LogInformation($"{DateTime.Now} | End sending updated user to message broker, id {userId}");

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
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return false;
            }

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

        private async Task<UserResponseModel> MapUserToResponse(User user)
        {
            return new UserResponseModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                IsAdmin = await _userManager.IsInRoleAsync(user, "Admin")
            };
        }

        private async Task<IEnumerable<UserResponseModel>> MapUsersToResponse(IEnumerable<User> users)
        {
            var usersResponse = new List<UserResponseModel>();
            foreach (var user in users)
            {
                usersResponse.Add(new UserResponseModel
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        IsAdmin = await _userManager.IsInRoleAsync(user, "Admin")
                    }
                );
            }

            return usersResponse;
        }

    }
}