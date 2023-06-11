using Microsoft.EntityFrameworkCore;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;

namespace TastyCook.RecipesAPI.Services
{
    public class RecipeService
    {
        private readonly RecipesContext _db;
        private readonly FileService _fileService;

        public RecipeService(RecipesContext db, FileService filesService)
        {
            _db = db;
            _fileService = filesService;
        }

        public int GetAllCount(string searchValue, IEnumerable<string> filters, Localization localization)
        {
            int count = GetRecipesCountByFilters(searchValue, filters, localization);
            return count;
        }

        public int GetAllUserCount(string email, string searchValue, IEnumerable<string> filters, Localization localization)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return 0;
            }

            int count = GetRecipesCountByFilters(searchValue, filters, localization, email);
            return count;
        }

        public int GetAllUserLikedCount(string email, Localization localization)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return 0;
            }

            int count = GetLikedRecipesCountByFilters(localization, email);
            return count;
        }

        public IEnumerable<Recipe> GetAll(GetAllRecipesRequest request)
        {
            var recipesQuery = GetRecipesByFiltersQuery(request.SearchValue, request.Filters, request.Localization, "");
            var recipes = GetRecipesByPagination(recipesQuery, request.Limit, request.Offset)
                .Select(r => new Recipe(r)
                {
                    Categories = r.Categories.Where(c => c.Localization == request.Localization).ToList()
                }).ToList();

            return recipes;
        }

        public IEnumerable<Recipe> GetUserRecipes(string email, GetAllRecipesRequest request)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return Enumerable.Empty<Recipe>();
            }

            var recipesQuery = GetRecipesByFiltersQuery(request.SearchValue, request.Filters, request.Localization, "", email);
            var recipes = GetRecipesByPagination(recipesQuery, request.Limit, request.Offset).Select(r => new Recipe(r)
            {
                Categories = r.Categories.Where(c => c.Localization == request.Localization).ToList()
            }).ToList();

            return recipes;
        }

        public IEnumerable<Recipe> GetUserLikedRecipes(string email, GetAllRecipesRequest request)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return Enumerable.Empty<Recipe>();
            }

            var recipesQuery = GetRecipesByFiltersQuery("", Enumerable.Empty<string>(), request.Localization, user.Id);

            if (request.Localization != Localization.None)
            {
               return GetRecipesByPagination(recipesQuery, request.Limit, request.Offset).Select(r =>
                    new Recipe(r)
                    {
                        Categories = r.Categories.Where(c => c.Localization == request.Localization).ToList()
                    }).ToList();
            }

            return GetRecipesByPagination(recipesQuery, request.Limit, request.Offset).ToList();
        }

        public List<Recipe> GetRecipesByProducts(GetRecipesByProductListRequest request)
        {
            var recipesQuery = GetRecipesByFiltersQuery(request.SearchValue, request.Filters, request.Localization, "").ToList();
            List<Recipe> recipesList = new List<Recipe>();
            for (var i = 0; i < recipesQuery.Count; i++)
            {
                var recipe = recipesQuery[i];
                var checkAllProducts = request.Products.All(pn => recipe.RecipeProducts.Any(rp => rp.Product.Name == pn));
                if (checkAllProducts)
                {
                    recipesList.Add(recipe);
                }
            }

            var recipesListQuery = recipesList.AsQueryable()
                .Include(r => r.Categories)
                .Include(r => r.RecipeProducts)
                .ThenInclude(r => r.Product);

            if (request.Localization != Localization.None)
            {
                return GetRecipesByPagination(recipesListQuery, request.Limit, request.Offset)
                    .Select(r => new Recipe(r)
                    {
                        Categories = r.Categories.Where(c => c.Localization == request.Localization).ToList(),
                        RecipeProducts = r.RecipeProducts.Where(rp => rp.Product.Localization == request.Localization).ToList()
                    }).ToList();
            }
            

            return GetRecipesByPagination(recipesListQuery, request.Limit, request.Offset).ToList();
        }

        public Recipe GetById(int id, Localization localization)
        {
            var recipe = _db.Recipes.Include(r => r.Categories)
                .Include(r => r.RecipeUsers)
                .FirstOrDefault(r => r.Id == id);

            if (localization != Localization.None)
            {
                recipe.Categories = recipe.Categories.Where(c => c.Localization == localization);
            }

            return recipe ?? throw new Exception("There is no such recipe");
        }

        public Recipe Add(RecipeModel recipe, string userEmail)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
            var categories = _db.Categories.Where(c => recipe.Categories.Any(rc => rc == c.Name)).Distinct().ToList();

            var dbRecipe = new Recipe
            {
                Name = recipe.Title,
                Description = recipe.Description,
                Categories = categories,
                Localization = recipe.Localization,
                UserId = user.Id,
            };

            _db.Recipes.Add(dbRecipe);
            _db.SaveChanges();

            var products = _db.Products.Where(p => recipe.Products.Any(rp => rp == p.Name)).Distinct().ToList();
            var recipeProducts = products.Select(p => new RecipeProduct()
                { ProductId = p.Id, RecipeId = dbRecipe.Id, Amount = "1 item" });

            _db.RecipeProducts.AddRange(recipeProducts);
            _db.SaveChanges();

            var createdRecipe = _db.Recipes.Include(r => r.Categories)
                .Include(r => r.RecipeProducts)
                .ThenInclude(r => r.Product)
                .FirstOrDefault(r => r.Id == dbRecipe.Id);

            return createdRecipe;
        }

        public void Update(RecipeModel recipe, string userEmail)
        {
            //var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
            var recipeDb = _db.Recipes.Include(r => r.Categories)
                .FirstOrDefault(r => r.Id == recipe.Id);

            recipeDb.Description = string.IsNullOrWhiteSpace(recipe.Description) ? recipeDb.Description : recipe.Description;
            recipeDb.Name = string.IsNullOrWhiteSpace(recipe.Title) ? recipeDb.Name : recipe.Title;
            recipeDb.Localization = recipe.Localization;

            var categories = _db.Categories.Where(c => recipe.Categories.Any(rc => rc == c.Name)).Distinct()
                .ToList();

            recipeDb.Categories = categories;
            _db.SaveChanges();

            var recipeProductsToDelete = _db.RecipeProducts.Where(rp => rp.RecipeId == recipe.Id);
            _db.RecipeProducts.RemoveRange(recipeProductsToDelete);

            var products = _db.Products.Where(p => recipe.Products.Any(rp => rp == p.Name)).Distinct().ToList();
            var recipeProducts = products.Select(p => new RecipeProduct()
                { ProductId = p.Id, RecipeId = recipe.Id, Amount = "1 item" });

            _db.RecipeProducts.AddRange(recipeProducts);
            _db.SaveChanges();
        }

        public async Task UpdateImage(int id, IFormFile data)
        {
            //var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
            var recipeDb = _db.Recipes.FirstOrDefault(r => r.Id == id);

            var fileName = $"recipe{id}Image{Path.GetExtension(data.FileName)}";
            var fileUrl = "https://tastycookfilestorage.blob.core.windows.net/recipeimages/" + fileName;
            var fileUploadResult = await _fileService.UploadFile(data, fileName);
            if (fileUploadResult)
            {
                recipeDb.ImageUrl = fileUrl;
                await _db.SaveChangesAsync();
            }
        }

        public void DeleteById(int id, string userEmail)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
            Recipe recipeToDelete;
            if (user.Role == "Admin")
            {
                recipeToDelete = _db.Recipes.FirstOrDefault(r => r.Id == id);
            }
            else
            {
                recipeToDelete = _db.Recipes.FirstOrDefault(r => r.Id == id && r.UserId == user.Id);
            }

            if (recipeToDelete == null)
            {
                throw new Exception("You don't have access to delete this recipe");
            }

            var recipesUsers = _db.RecipeUsers.Where(ru => ru.RecipeId == id).ToList();
            _db.RecipeUsers.RemoveRange(recipesUsers);
            var recipesProducts = _db.RecipeProducts.Where(rp => rp.RecipeId == id).ToList();
            _db.RecipeProducts.RemoveRange(recipesProducts);
            _db.SaveChanges();


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

        private IQueryable<Recipe> GetRecipesByFiltersQuery(string searchValue,
            IEnumerable<string> filters, Localization localization, string likedUserId, string email = "")
        {
            IQueryable<Recipe> recipes = _db.Recipes
                .Include(r => r.Categories)
                .Include(r => r.RecipeUsers)
                .Include(r => r.RecipeProducts)
                .ThenInclude(rp => rp.Product);

            if (localization != Localization.None)
            {
                recipes = recipes.Where(r => r.Localization == localization);
            }

            if (!string.IsNullOrWhiteSpace(likedUserId))
            {
                recipes = recipes.Where(r => r.RecipeUsers.Any(ru => ru.UserId == likedUserId && ru.IsUserLiked));
            }

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

        private int GetRecipesCountByFilters(string searchValue, IEnumerable<string> filters, Localization localization, string email = "")
        {
            int count = 0;
            IQueryable<Recipe> recipes = _db.Recipes.Include(r => r.Categories);

            if (localization != Localization.None)
            {
                recipes = recipes.Where(r => r.Localization == localization);
            }

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

        private int GetLikedRecipesCountByFilters(Localization localization, string email)
        {
            var user = _db.Users.First(u => u.Email == email);

            if (!string.IsNullOrEmpty(email))
            {
                return localization != Localization.None ? 
                    _db.Recipes.Count(r => r.Localization == localization && r.RecipeUsers.Any(x => x.UserId == user.Id && x.IsUserLiked)) :
                    _db.Recipes.Count(r => r.RecipeUsers.Any(x => x.UserId == user.Id && x.IsUserLiked));
            }

            return 0;
        }
    }
}
