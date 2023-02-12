using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TastyCook.RecepiesAPI.Models;

namespace TastyCook.RecepiesAPI.Controllers
{
    [ApiController]
    [Route("/recipe")]
    public class RecipeController : ControllerBase
    {
        private readonly RecipeService _recipeService;

        public RecipeController(RecipeService recipeService)
        {
            _recipeService= recipeService;
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
        public ActionResult Add(Recipe recipe)
        {
            try
            {
                _recipeService.Add(recipe);
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
        public ActionResult Update(Recipe recipe)
        {
            try
            {
                _recipeService.Update(recipe);
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
