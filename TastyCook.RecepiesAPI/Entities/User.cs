using TastyCook.RecepiesAPI.Entities;

namespace TastyCook.RecepiesAPI.Models;

public class User
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    //public IEnumerable<RecipeUser> RecipeUsers { get; set; }
    public virtual IEnumerable<Recipe> Recipes { get; set; }
}