using System.ComponentModel.DataAnnotations;
using TastyCook.RecipesAPI.Entities;

namespace TastyCook.RecipesAPI.Entities;

public class User
{
    [MaxLength(225)]
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public IEnumerable<Recipe> Recipes { get; set; }
    public IEnumerable<RecipeUser> RecipeUsers { get; set; }
    public IEnumerable<Comment> Comments { get; set; }
}