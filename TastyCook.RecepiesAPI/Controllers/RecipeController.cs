using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TastyCook.RecepiesAPI.Models;
using TastyCookBroker;

namespace TastyCook.RecepiesAPI.Controllers
{
    [ApiController]
    [Route("/recipe")]
    public class RecipeController : ControllerBase
    {
        private readonly RecipeService _recipeService;
        private readonly IPublishEndpoint _publishEndpoint;

        public RecipeController(RecipeService recipeService, IPublishEndpoint publishEndpoint)
        {
            _recipeService= recipeService;
            _publishEndpoint= publishEndpoint;
        }

        // GET: RecipeController
        [HttpGet]
        [Route("GetAll")]
        public IEnumerable<Recipe> GetAll()
        {
            return _recipeService.GetAll();
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
        public async Task<ActionResult> Add(Recipe recipe)
        {
            try
            {
                _recipeService.Add(recipe);
                await _publishEndpoint.Publish(new Contracts.RecipeCreated(recipe.Id));
                return Ok();
            }
            catch (Exception exc)
            {
                return NotFound(exc);
            }
        }

        // POST: RecipeController/Edit/5
        [HttpPost("Update")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(Recipe recipe)
        {
            try
            {
                _recipeService.Update(recipe);
                await _publishEndpoint.Publish(new Contracts.RecipeUpdated(recipe.Id));
                return Ok();
            }
            catch (Exception exc)
            {
                return NotFound(exc);
            }
        }

        // POST: RecipeController/Delete/5
        [HttpPost("DeleteById")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteById(int id)
        {
            try
            {
                _recipeService.DeleteById(id);
                return Ok();
            }
            catch (Exception exc)
            {
                return NotFound(exc);
            }
        }
    }
}
