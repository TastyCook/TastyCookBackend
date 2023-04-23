using Microsoft.AspNetCore.Mvc;
using TastyCook.RecepiesAPI.Models;

namespace TastyCook.RecepiesAPI
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
            //var recipes = _db.Recipes.Where(r => r.RecipeUsers.In);
            //return recipes;
            throw new NotImplementedException();
        }

        public Recipe GetById(int id)
        {
            return _db.Recipes.FirstOrDefault(r => r.Id == id);
        }

        public void Add(Recipe recipe)
        {
            _db.Recipes.Add(recipe);
            _db.SaveChanges();
        }

        public void Update(Recipe recipe)
        {
            _db.Recipes.Update(recipe);
            _db.SaveChanges();
        }

        public void DeleteById(int id)
        {
            var recipeToDelete = _db.Recipes.FirstOrDefault(r =>r.Id == id);
            if (recipeToDelete != null)
            {
                _db.Recipes.Remove(recipeToDelete);
                _db.SaveChanges();
            }
        }
    }
}
