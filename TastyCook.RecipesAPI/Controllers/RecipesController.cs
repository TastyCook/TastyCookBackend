using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;
using TastyCook.RecipesAPI.Services;
using TastyCook.UsersAPI.Models;

namespace TastyCook.RecipesAPI.Controllers
{
    [ApiController]
    [Route("/api/recipes")]
    [Authorize]
    public class RecipesController : ControllerBase
    {
        private readonly ILogger<RecipesController> _logger;
        private readonly RecipeService _recipeService;
        private readonly IPublishEndpoint _publishEndpoint;

        public RecipesController(RecipeService recipeService,
            IPublishEndpoint publishEndpoint,
            ILogger<RecipesController> logger)
        {
            _recipeService = recipeService;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public IActionResult GetAll(int limit, int offset)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start getting all recipes");

                var recipes = _recipeService.GetAll(limit, offset);
                var totalRecipes = _recipeService.GetAllCount();

                var recipesResponse = new GetRecipesResponse()
                {
                    Recipes = recipes,
                    TotalPagesWithCurrentLimit = totalRecipes / limit < 1 ? 1 : totalRecipes / limit
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

        // TODO: TBD recipes for user recommendations
        //[HttpGet]
        //[Route("GetAllByUser")]
        //public IEnumerable<Recipe> GetAllByUser()
        //{
        //    string userId = User.Identity.Name;

        //    return _recipeService.GetAllByUser();
        //}

        [HttpGet]
        [Route("by-user")]
        public IActionResult GetAllByUser(int limit, int offset)
        {
            try
            {
                string userEmail = User.Identity.Name;
                _logger.LogInformation($"{DateTime.Now} | Start getting all recipes by user {userEmail}");

                var recipes = _recipeService.GetUserRecipes(userEmail, limit, offset);
                var totalRecipes = _recipeService.GetAllUserCount();

                var recipesResponse = new GetRecipesResponse()
                {
                    Recipes = recipes,
                    TotalPagesWithCurrentLimit = totalRecipes / limit < 1 ? 1 : totalRecipes / limit
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
        //[Route("")]
        public ActionResult<Recipe> GetById(int id)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start getting recipe by id {id}");
                var recipe = _recipeService.GetById(id);
                _logger.LogInformation($"{DateTime.Now} | End getting recipe by id {id}");
                return recipe;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                return StatusCode(500, exc.Message);
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
                _recipeService.Add(recipe, userEmail);
                _logger.LogInformation($"{DateTime.Now} | End creating new recipe, title {recipe.Title}");

                return Ok();
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
        public async Task<ActionResult> IncrementLikes(int id, [FromBody] ChangeLikesModel model)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start updating recipe likes, id: {id}");
                _recipeService.UpdateLikes(id, model.IsPositive);
                _logger.LogInformation($"{DateTime.Now} | End updating new recipe, id: {id}");

                return Ok();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                return StatusCode(500, exc.Message);
            }
        }
    }
}
