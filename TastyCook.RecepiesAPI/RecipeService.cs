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

        // GET: RecipeService
        public IEnumerable<Recipe> GetAll()
        {
            return _db.Recipes.ToList();
        }

        // GET: RecipeService/Details/5
        public Recipe GetById(int id)
        {
            return _db.Recipes.FirstOrDefault(r => r.Id == id);
        }

        // POST: RecipeService/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public void Add(Recipe recipe)
        {
            _db.Recipes.Add(recipe);
            _db.SaveChanges();
        }

        // POST: RecipeService/Edit/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public void Update(Recipe recipe)
        {
            _db.Recipes.Update(recipe);
            _db.SaveChanges();
        }

        // POST: RecipeService/Delete/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
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
