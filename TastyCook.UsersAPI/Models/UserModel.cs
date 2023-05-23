using System.ComponentModel.DataAnnotations;

namespace TastyCook.UsersAPI.Models
{
    public class UserModel
    {
        public string? Username { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
