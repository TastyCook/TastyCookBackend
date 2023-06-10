using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;
using TastyCook.RecipesAPI.Services;

namespace TastyCook.RecipesAPI.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ILogger<CategoriesController> _logger;
    private readonly CategoriesService _categoriesService;

    public CategoriesController(CategoriesService categoriesService,
        ILogger<CategoriesController> logger)
    {
        _categoriesService = categoriesService;
        _logger = logger;
    }

    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    public IActionResult GetAll()
    {
        try
        {
            _logger.LogInformation($"{DateTime.Now} | Start getting all categories");
            var categories = _categoriesService.GetAll();
            _logger.LogInformation($"{DateTime.Now} | End getting all recipes");

            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [Route("")]
    public IActionResult AddCategory([FromBody] CategoryModel model)
    {
        try
        {
            _logger.LogInformation($"{DateTime.Now} | Start adding new category");
            _categoriesService.Add(new Category() { Name = model.Name, Localization = model.Localization});
            _logger.LogInformation($"{DateTime.Now} | End adding new category");

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPatch]
    [Route("{id}")]
    public IActionResult UpdateCategory(int id, [FromBody] CategoryModel model)
    {
        try
        {
            _logger.LogInformation($"{DateTime.Now} | Start adding new category");
            model.Id = id;
            _categoriesService.Update(new Category() { Id = model.Id, Name = model.Name, Localization = model.Localization });
            _logger.LogInformation($"{DateTime.Now} | End adding new category");

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete]
    [Route("{id}")]
    public IActionResult DeleteCategory(int id)
    {
        try
        {
            _logger.LogInformation($"{DateTime.Now} | Start deleting category");
            _categoriesService.DeleteById(id);
            _logger.LogInformation($"{DateTime.Now} | End deleting category");

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}