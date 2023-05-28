using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;

namespace TastyCook.RecipesAPI
{
    public class RecipeService
    {
        private readonly RecipesContext _db;

        public RecipeService(RecipesContext db)
        {
            _db = db;
        }

        public IEnumerable<Recipe> GetAll(int limit, int offset)
        {
            var recipes = _db.Recipes.Skip(offset).Take(limit).ToList();
            return recipes;
        }

        public IEnumerable<Recipe> GetUserRecipes(string email)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return Enumerable.Empty<Recipe>();
            }

            var recipes = _db.Recipes.Where(r => r.UserId == user.Id).ToList();
            return recipes;
        }

        public Recipe GetById(int id)
        {
            return _db.Recipes.FirstOrDefault(r => r.Id == id);
        }

        public void Add(Recipe recipe, string userEmail)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
            recipe.UserId = user.Id;
            _db.Recipes.Add(recipe);
            _db.SaveChanges();
        }

        public void Update(RecipeModel recipe, string userEmail)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
            var recipeDb = _db.Recipes.Find(recipe.Id);
            recipeDb.Description = string.IsNullOrWhiteSpace(recipe.Description) ? recipeDb.Description : recipe.Description;
            recipeDb.Name = string.IsNullOrWhiteSpace(recipe.Title) ? recipeDb.Name : recipe.Title;
            _db.SaveChanges();
        }

        public void DeleteById(int id, string userEmail)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
            var recipeToDelete = _db.Recipes.FirstOrDefault(r =>r.Id == id && r.UserId == user.Id);
            if (recipeToDelete != null)
            {
                _db.Recipes.Remove(recipeToDelete);
                _db.SaveChanges();
            }
        }

        public void UpdateLikes(int id, bool isPositive)
        {
            var recipeDb = _db.Recipes.Find(id);
            recipeDb.Likes += isPositive ? 1 : -1;
            _db.SaveChanges();
        }
    }
}
