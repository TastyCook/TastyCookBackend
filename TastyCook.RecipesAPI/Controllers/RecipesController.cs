using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;
//using TastyCookBroker;

namespace TastyCook.RecipesAPI.Controllers
{
    [ApiController]
    [Route("/api/recipes")]
    public class RecipesController : ControllerBase
    {
        private readonly RecipeService _recipeService;
        private readonly IPublishEndpoint _publishEndpoint;

        public RecipesController(RecipeService recipeService, IPublishEndpoint publishEndpoint)
        {
            _recipeService = recipeService;
            _publishEndpoint = publishEndpoint;
        }

        // GET: RecipeController
        [HttpGet]
        [AllowAnonymous]
        [Route("GetAll")]
        public IEnumerable<Recipe> GetAll(int limit, int offset)
        {
            return _recipeService.GetAll(limit, offset);
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
        public IEnumerable<Recipe> GetAllByUser()
        {
            string userEmail= User.Identity.Name;

            return _recipeService.GetUserRecipes(userEmail);
        }

        // GET: RecipeController/Details/5
        [HttpGet("GetById/{id}")]
        public ActionResult<Recipe> GetById(int id)
        {
            try
            {
                return _recipeService.GetById(id);
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
                string userEmail = User.Identity.Name;
                _recipeService.Add(new Recipe() { Name = recipe.Title, Description = recipe.Description }, userEmail);
                //await _publishEndpoint.Publish(new Contracts.Contracts.RecipeItemCreated(recipe.Id, recipe.Title, recipe.Description));
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
                string userEmail = User.Identity.Name;
                _recipeService.Update(recipe, userEmail);

                //await _publishEndpoint.Publish(new Contracts.RecipeUpdated(recipe.Id));
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
                string userEmail = User.Identity.Name;
                _recipeService.DeleteById(id, userEmail);
                return Ok();
            }
            catch (Exception exc)
            {
                return NotFound(exc);
            }
        }
    }
}
