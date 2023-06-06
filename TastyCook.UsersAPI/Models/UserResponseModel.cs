namespace TastyCook.UsersAPI.Models;

public class UserResponseModel
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string UserName{ get; set; }
    public bool IsAdmin { get; set; }
}