using System.ComponentModel.DataAnnotations;

namespace TastyCook.UsersAPI.Models
{
    public class UserModel
    {
        //[Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }

    public class AuthModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
