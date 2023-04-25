using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Google.Apis.Auth;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using TastyCook.UsersAPI.Entities;
using TastyCook.UsersAPI.Models;
using static MassTransit.ValidationResultExtensions;

namespace TastyCook.UsersAPI.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IPublishEndpoint _publishEndpoint;


        public UserController(UserManager<User> userManager, IConfiguration configuration, IPublishEndpoint publishEndpoint)
        {
            _userManager = userManager;
            _configuration = configuration;
            _publishEndpoint = publishEndpoint;
        }

        public class AuthenticateRequest
        {
            [Required] public string IdToken { get; set; }
        }

        [AllowAnonymous]
        [HttpPost]

        [Route("googleAuthenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest data)
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();

            // Change this to your google client ID
            settings.Audience = new List<string>()
                { "949493455412-lab5qmkch79a6ilg7u9vrin4lbhe024q.apps.googleusercontent.com" };

            GoogleJsonWebSignature.Payload
                payload = GoogleJsonWebSignature.ValidateAsync(data.IdToken, settings).Result;
            return Ok(new { AuthToken = await CreateTokenAsync(new UserModel() { Email = payload.Email}) });
        }

        [HttpGet]
        [Route("test")]
        [Authorize]
        public string Test()
        {
            string userName = User.Identity.Name;
            return "Hello";
        }

        [HttpPost]
        [Route("register")]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Register([FromBody] UserModel user)
        {
            User identityUser = new User { Email = user.Email, UserName = user.Email };
            var result = await _userManager.CreateAsync(identityUser, user.Password);
            if (result.Succeeded)
            {
                var userFromDb = await _userManager.FindByNameAsync(user.Email);
                await _publishEndpoint.Publish(new Contracts.Contracts.UserItemCreated(userFromDb.Id, user.Email, user.Password));
            }
            return !result.Succeeded ? new BadRequestObjectResult(result) : StatusCode(201);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LogIn([FromBody] UserModel model)
        {
            return !await ValidateUserAsync(model)
                ? Unauthorized()
                : Ok(new { Token = await CreateTokenAsync(model) });
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
            if (changePasswordModel?.RepeatNewPassword != changePasswordModel?.RepeatNewPassword)
            {
                return BadRequest("Passwords must be same.");
            }

            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordModel.CurrentPassword, changePasswordModel.NewPassword);

            await _publishEndpoint.Publish(new Contracts.Contracts.UserItemUpdated(user.Id, user.Email, changePasswordModel.NewPassword));

            if (result.Succeeded)
            {
                // Password changed successfully
                return Ok();
            }

            return BadRequest(result.Errors);
        }

        [HttpPut]
        [Route("changeEmail")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailModel changeEmailModel)
        {
            string userName = User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(userName);
            user.Email = changeEmailModel.NewEmail;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await _publishEndpoint.Publish(new Contracts.Contracts.UserItemUpdated(user.Id, changeEmailModel.NewEmail, null));
                return Ok();
            }

            return StatusCode(500, result.Errors.First());
        }

        private async Task<bool> ValidateUserAsync(UserModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
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
            var user = await _userManager.FindByNameAsync(model.Email);
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
