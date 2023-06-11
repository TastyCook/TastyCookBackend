using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;
using TastyCook.RecipesAPI.Services;

namespace TastyCook.RecipesAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class RecipesController : ControllerBase
    {
        private readonly ILogger<RecipesController> _logger;
        private readonly RecipeService _recipeService;
        private readonly UserService _userService;
        private readonly ProductService _productService;

        public RecipesController(RecipeService recipeService,
            ILogger<RecipesController> logger,
            UserService userService,
            ProductService productService)
        {
            _productService = productService;
            _recipeService = recipeService;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public IActionResult GetAll([FromQuery] GetAllRecipesRequest request)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start getting all recipes");

                var recipes = _recipeService.GetAll(request);
                var totalRecipes = _recipeService.GetAllCount(request.SearchValue, request.Filters, request.Localization);
                var totalPagesWithCurrentLimit = int.MaxValue;
                if (request.Limit.HasValue && request.Limit > 0)
                {
                    var pages = GetFlooredInt(totalRecipes, request.Limit.Value);
                    totalPagesWithCurrentLimit = pages < 1 ? 1 : pages;
                }

                var recipesResponse = new GetRecipesResponse()
                {
                    Recipes = MapRecipesToResponse(recipes, User.Identity.Name),
                    TotalPagesWithCurrentLimit = totalPagesWithCurrentLimit
                };

                _logger.LogInformation($"{DateTime.Now} | End getting all recipes");

                return Ok(recipesResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("recommendations")]
        public IActionResult GetRecipesByProductList([FromQuery] GetRecipesByProductListRequest request)
        {
            try
            {
                var recipes = _recipeService.GetRecipesByProducts(request);
                var recipeResult = MapRecipesToResponse(recipes, "");

                return Ok(recipeResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("by-user")]
        public IActionResult GetAllByUser([FromQuery] GetAllRecipesRequest request)
        {
            try
            {
                string userEmail = User.Identity.Name;
                _logger.LogInformation($"{DateTime.Now} | Start getting all recipes by user {userEmail}");

                var recipes = _recipeService.GetUserRecipes(userEmail, request);
                var totalRecipes = _recipeService.GetAllUserCount(userEmail, request.SearchValue, request.Filters, request.Localization);
                var totalPagesWithCurrentLimit = int.MaxValue;
                if (request.Limit.HasValue && request.Limit > 0)
                {
                    var pages = GetFlooredInt(totalRecipes, request.Limit.Value);
                    totalPagesWithCurrentLimit = pages < 1 ? 1 : pages;
                }

                var recipesResponse = new GetRecipesResponse()
                {
                    Recipes = MapRecipesToResponse(recipes, User.Identity.Name),
                    TotalPagesWithCurrentLimit = totalPagesWithCurrentLimit
                };

                _logger.LogInformation($"{DateTime.Now} | End getting all recipes by user {userEmail}");

                return Ok(recipesResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("by-user/liked")]
        public IActionResult GetAllLikedByUser([FromQuery] GetAllRecipesRequest request)
        {
            try
            {
                string userEmail = User.Identity.Name;
                _logger.LogInformation($"{DateTime.Now} | Start getting all recipes by user {userEmail}");

                var recipes = _recipeService.GetUserLikedRecipes(userEmail, request);
                var totalRecipes = _recipeService.GetAllUserLikedCount(userEmail, request.Localization);
                var totalPagesWithCurrentLimit = int.MaxValue;
                if (request.Limit.HasValue && request.Limit > 0)
                {
                    var pages = GetFlooredInt(totalRecipes, request.Limit.Value);
                    totalPagesWithCurrentLimit = pages < 1 ? 1 : pages;
                }

                var recipesResponse = new GetRecipesResponse()
                {
                    Recipes = MapRecipesToResponse(recipes, User.Identity.Name),
                    TotalPagesWithCurrentLimit = totalPagesWithCurrentLimit
                };

                _logger.LogInformation($"{DateTime.Now} | End getting all recipes by user {userEmail}");

                return Ok(recipesResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        //[Route("")]
        public ActionResult<RecipeModel> GetById(int id, Localization localization)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start getting recipe by id {id}");
                var recipe = _recipeService.GetById(id, localization);
                var recipeResponse = MapRecipeToResponse(recipe, User.Identity.Name);
                _logger.LogInformation($"{DateTime.Now} | End getting recipe by id {id}");
                return recipeResponse;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                return StatusCode(500, exc.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("count")]
        public IActionResult GetAllCount([FromQuery] GetAllRecipesRequest request)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start getting all recipes");
                var totalRecipes = _recipeService.GetAllCount(request.SearchValue, request.Filters, request.Localization);
                _logger.LogInformation($"{DateTime.Now} | End getting all recipes");

                return Ok(totalRecipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("count/by-user")]
        public IActionResult GetAllCountByUser([FromQuery] GetAllRecipesRequest request)
        {
            try
            {
                string userEmail = User.Identity.Name;
                _logger.LogInformation($"{DateTime.Now} | Start getting all recipes by user {userEmail}");
                var totalRecipes = _recipeService.GetAllUserCount(userEmail, request.SearchValue, request.Filters, request.Localization);
                _logger.LogInformation($"{DateTime.Now} | End getting all recipes by user {userEmail}");

                return Ok(totalRecipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Add([FromBody] RecipeModel recipe)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start creating new recipe, title {recipe.Title}");
                string userEmail = User.Identity.Name;
                var createdRecipe = _recipeService.Add(recipe, userEmail);
                var recipeResult = MapRecipeToResponse(createdRecipe, userEmail);
                _logger.LogInformation($"{DateTime.Now} | End creating new recipe, title {recipe.Title}");

                return Ok(recipeResult);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                return StatusCode(500, exc.Message);
            }
        }

        [HttpPatch("")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(RecipeModel recipe)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start updating new recipe, id: {recipe.Id}");
                string userEmail = User.Identity.Name;
                _recipeService.Update(recipe, userEmail);
                _logger.LogInformation($"{DateTime.Now} | End updating new recipe, id: {recipe.Id}");
                
                return Ok();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                return StatusCode(500, exc.Message);
            }
        }

        [HttpDelete("{id}")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteById(int id)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start deleting recipe by id {id}");
                string userEmail = User.Identity.Name;
                _recipeService.DeleteById(id, userEmail);
                _logger.LogInformation($"{DateTime.Now} | End deleting recipe by id {id}");

                return Ok();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                return StatusCode(500, exc.Message);
            }
        }


        [HttpPatch("{id}/likes")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeLikeForRecipe(int id)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start updating recipe likes, id: {id}");
                await _recipeService.UpdateLikesAsync(id, User.Identity.Name);
                _logger.LogInformation($"{DateTime.Now} | End updating new recipe, id: {id}");

                return Ok();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                return StatusCode(500, exc.Message);
            }
        }

        [HttpGet("{id}/image")]
        [AllowAnonymous]
        public ActionResult<RecipeModel> GetImageById(int id)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start getting recipe image by id {id}");
                var recipe = _recipeService.GetById(id, Localization.None);
                
                _logger.LogInformation($"{DateTime.Now} | End getting recipe image by id {id}");
                return File(recipe.ImageUrl, "image/png");
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                return StatusCode(500, exc.Message);
            }
        }

        [HttpPatch]
        [Route("{id}/image")]
        public async Task<IActionResult> Upload(int id, IFormFile data)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start updating image, id: {id}");
                await _recipeService.UpdateImage(id, data);
                _logger.LogInformation($"{DateTime.Now} | End updating image, id: {id}");

                return Ok();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                return StatusCode(500, exc.Message);
            }
        }

        private IEnumerable<RecipeModel> MapRecipesToResponse(IEnumerable<Recipe> recipes, string email)
        {
            var user = _userService.GetByEmail(email);

            var responseRecipes = recipes.Select(r => new RecipeModel()
            {
                Id = r.Id,
                Title = r.Name,
                Description = r.Description,
                Categories = r.Categories?.Select(c => c.Name),
                Likes = r.Likes,
                UserId = r.UserId,
                IsUserLiked = r.RecipeUsers?.FirstOrDefault(x => x.UserId == user?.Id)?.IsUserLiked ?? false,
                Localization = r.Localization,
                ImageUrl = r.ImageUrl,
                Products = r.RecipeProducts?.Select(rp => rp.Product?.Name).ToList()
            });

            return responseRecipes;
        }

        private RecipeModel MapRecipeToResponse(Recipe recipe, string email)
        {
            var user = _userService.GetByEmail(email);

            var responseRecipe = new RecipeModel()
            {
                Id = recipe.Id,
                Title = recipe.Name,
                Description = recipe.Description,
                Categories = recipe.Categories?.Select(c => c.Name),
                Likes = recipe.Likes,
                UserId = recipe.UserId,
                IsUserLiked = recipe.RecipeUsers?.FirstOrDefault(x => x.UserId == user?.Id)?.IsUserLiked ?? false,
                Localization = recipe.Localization,
                ImageUrl = recipe.ImageUrl,
                Products = recipe.RecipeProducts?.Select(rp => rp.Product?.Name).ToList()
            };

            return responseRecipe;
        }

        private int GetFlooredInt(int a, int b) => (a + b - 1) / b;
    }
}
