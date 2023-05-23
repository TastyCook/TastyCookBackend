using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;

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

        // GET: RecipeController
        [HttpGet]
        [AllowAnonymous]
        [Route("GetAll")]
        public IActionResult GetAll(int limit, int offset)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start getting all recipes");
                var recipes = _recipeService.GetAll(limit, offset);
                _logger.LogInformation($"{DateTime.Now} | End getting all recipes");

                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        // TO DO: TBD recipes for user recommendations
        //// GET: RecipeController
        //[HttpGet]
        //[Route("GetAllByUser")]
        //public IEnumerable<Recipe> GetAllByUser()
        //{
        //    string userId = User.Identity.Name;

        //    return _recipeService.GetAllByUser();
        //}

        // GET: RecipeController
        [HttpGet]
        [Route("GetAllByUser")]
        public IActionResult GetAllByUser()
        {
            try
            {
                string userEmail = User.Identity.Name;
                _logger.LogInformation($"{DateTime.Now} | Start getting all recipes by user {userEmail}");
                var recipes = _recipeService.GetUserRecipes(userEmail);
                _logger.LogInformation($"{DateTime.Now} | End getting all recipes by user {userEmail}");

                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        // GET: RecipeController/Details/5
        [HttpGet("GetById/{id}")]
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
                return NotFound(exc);
            }
        }

        // POST: RecipeController/Create
        [HttpPost("Add")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Add([FromBody] RecipeModel recipe)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now} | Start creating new recipe, title {recipe.Title}");
                string userEmail = User.Identity.Name;
                _recipeService.Add(new Recipe() { Name = recipe.Title, Description = recipe.Description }, userEmail);
                _logger.LogInformation($"{DateTime.Now} | End creating new recipe, title {recipe.Title}");

                return Ok();
            }
            catch (Exception exc)
            {
                return NotFound(exc);
            }
        }

        // POST: RecipeController/Edit/5
        [HttpPut("Update")]
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
                return NotFound(exc);
            }
        }

        // POST: RecipeController/Delete/5
        [HttpDelete("Delete")]
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
                return NotFound(exc);
            }
        }
    }
}
