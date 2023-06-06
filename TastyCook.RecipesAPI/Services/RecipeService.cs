using System.Linq;
using Microsoft.EntityFrameworkCore;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;

namespace TastyCook.RecipesAPI.Services
{
    public class RecipeService
    {
        private readonly RecipesContext _db;

        public RecipeService(RecipesContext db)
        {
            _db = db;
        }

        public int GetAllCount(string searchValue, IEnumerable<string> filters)
        {
            int count = GetRecipesCountByFilters(searchValue, filters);
            return count;
        }

        public int GetAllUserCount(string email, string searchValue, IEnumerable<string> filters)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return 0;
            }

            int count = GetRecipesCountByFilters(searchValue, filters, email);
            return count;
        }

        public int GetAllUserLikedCount(string email)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return 0;
            }

            int count = GetLikedRecipesCountByFilters(email);
            return count;
        }

        public IEnumerable<Recipe> GetAll(GetAllRecipesRequest request)
        {
            var recipesQuery = GetRecipesByFiltersQuery(request.SearchValue, request.Filters);
            var recipes = GetRecipesByPagination(recipesQuery, request.Limit, request.Offset);

            return recipes;
        }

        public IEnumerable<Recipe> GetUserRecipes(string email, GetAllRecipesRequest request)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return Enumerable.Empty<Recipe>();
            }

            var recipesQuery = GetRecipesByFiltersQuery(request.SearchValue, request.Filters, email);
            var recipes = GetRecipesByPagination(recipesQuery, request.Limit, request.Offset);

            return recipes;
        }

        public IEnumerable<Recipe> GetUserLikedRecipes(string email, GetAllRecipesRequest request)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return Enumerable.Empty<Recipe>();
            }

            var recipesQuery = GetRecipesByFiltersQuery("", Enumerable.Empty<string>(), "")
                .Where(r => r.RecipeUsers.Any(x => x.UserId == user.Id && x.IsUserLiked));

            var recipes = GetRecipesByPagination(recipesQuery, request.Limit, request.Offset);

            return recipes;
        }

        public Recipe GetById(int id)
        {
            var recipe = _db.Recipes.Include(r => r.Categories)
                .Include(r => r.RecipeUsers)
                .FirstOrDefault(r => r.Id == id);

            return recipe ?? throw new Exception("There is no such recipe");
        }

        public void Add(RecipeModel recipe, string userEmail)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
            var categories = _db.Categories.Where(c => recipe.Categories.Any(rc => rc == c.Name))
                .ToList();

            var dbRecipe = new Recipe
            {
                Name = recipe.Title,
                Description = recipe.Description,
                Categories = categories,
                UserId = user.Id
            };

            _db.Recipes.Add(dbRecipe);
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
            var recipeToDelete = _db.Recipes.FirstOrDefault(r => r.Id == id && r.UserId == user.Id);
            if (recipeToDelete != null)
            {
                _db.Recipes.Remove(recipeToDelete);
                _db.SaveChanges();
            }
        }

        public async Task UpdateLikesAsync(int id, string email)
        {
            var recipeDb = await _db.Recipes.FindAsync(id);
            var user = await _db.Users.FirstAsync(u => u.Email == email);
            var recipeUser = await _db.RecipeUsers.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.RecipeId == id);

            if (recipeUser != null)
            {
                recipeDb.Likes += recipeUser.IsUserLiked ? -1 : 1;
                recipeUser.IsUserLiked = !recipeUser.IsUserLiked;
            }
            else
            {
                recipeDb.Likes += 1;
                await _db.RecipeUsers.AddAsync(new RecipeUser()
                {
                    UserId = user.Id, RecipeId = recipeDb.Id, IsUserLiked = true
                });
            }

            await _db.SaveChangesAsync();
        }

        private IEnumerable<Recipe> GetRecipesByPagination(IQueryable<Recipe> recipes, int? limit, int? offset)
        {
            if (limit.HasValue && offset.HasValue)
            {
                recipes = recipes.Skip(offset.Value).Take(limit.Value);
            }
            else if (limit.HasValue && !offset.HasValue)
            {
                recipes = recipes.Take(limit.Value);
            }
            else if (!limit.HasValue && offset.HasValue)
            {
                recipes = recipes.Skip(offset.Value);
            }

            return recipes.ToList();
        }

        private IQueryable<Recipe> GetRecipesByFiltersQuery(string searchValue, IEnumerable<string> filters, string email = "")
        {
            IQueryable<Recipe> recipes = _db.Recipes
                .Include(r => r.Categories)
                .Include(r => r.RecipeUsers);

            if (!string.IsNullOrEmpty(email))
            {
                recipes = recipes.Where(r => r.User.Email == email);
            }

            if (!string.IsNullOrEmpty(searchValue) && filters.Any())
            {
                recipes = recipes.Where(r => (r.Name.Contains(searchValue) || r.Description.Contains(searchValue))
                                             && r.Categories.Any(c => filters.Any(f => f == c.Name)));
            }
            else if (!string.IsNullOrEmpty(searchValue) && !filters.Any())
            {
                recipes = recipes.Where(r => r.Name.Contains(searchValue) || r.Description.Contains(searchValue));
            }
            else if (filters.Any())
            {
                recipes = recipes.Where(r => r.Categories.Any(c => filters.Any(f => f == c.Name)));
            }

            return recipes;
        }

        private int GetRecipesCountByFilters(string searchValue, IEnumerable<string> filters, string email = "")
        {
            int count = 0;
            IQueryable<Recipe> recipes = _db.Recipes.Include(r => r.Categories);
            if (!string.IsNullOrEmpty(email))
            {
                recipes = recipes.Where(r => r.User.Email == email);
            }
            
            if (!string.IsNullOrEmpty(searchValue) && filters.Any())
            {
                count = recipes.Count(r => r.Name.Contains(searchValue) || r.Description.Contains(searchValue)
                    && r.Categories.Any(c => filters.Any(f => f == c.Name)));
            }
            else if (!string.IsNullOrEmpty(searchValue) && !filters.Any())
            {
                count = recipes.Count(r => r.Name.Contains(searchValue) || r.Description.Contains(searchValue));
            }
            else if (filters.Any())
            {
                count = recipes.Count(r => r.Categories.Any(c => filters.Any(f => f == c.Name)));
            }
            else
            {
                count = recipes.Count();
            }

            return count;
        }

        private int GetLikedRecipesCountByFilters(string email)
        {
            var user = _db.Users.First(u => u.Email == email);
            if (!string.IsNullOrEmpty(email))
            {
                return _db.Recipes.Count(r => r.RecipeUsers.Any(x => x.UserId == user.Id && x.IsUserLiked));
            }

            return 0;
        }
    }
}
