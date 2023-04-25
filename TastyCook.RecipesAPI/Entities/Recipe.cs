using System.ComponentModel.DataAnnotations.Schema;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;

namespace TastyCook.RecipesAPI.Entities
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //public IEnumerable<Products> Products { get; set; }
        public int Likes { get; set; }
        //public byte[] Image { get; set; }
        //public IEnumerable<RecipeUser> RecipeUsers { get; set; }
        public string UserId { get; set; }
        [NotMapped]
        public virtual User User { get; set; }
    }
}
