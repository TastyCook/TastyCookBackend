using TastyCook.RecipesAPI.Models;

namespace TastyCook.RecipesAPI.Entities
{
    public class RecipeUser
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}
