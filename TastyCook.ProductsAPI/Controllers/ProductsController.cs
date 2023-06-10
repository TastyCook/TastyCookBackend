using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TastyCook.ProductsAPI.Entities;
using TastyCook.ProductsAPI.Models;
using TastyCook.ProductsAPI.Services;

namespace TastyCook.ProductsAPI.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly ProductService _productService;

    public ProductsController(ProductService productService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    public IActionResult GetAll([FromQuery] ProductsRequest request)
    {
        try
        {
            _logger.LogInformation($"{DateTime.Now} | Start getting all products");
            var products = _productService.GetAll(request);
            var totalProducts = _productService.GetAllCount(request.SearchValue);
            var totalPagesWithCurrentLimit = int.MaxValue;

            if (request.Limit.HasValue && request.Limit > 0)
            {
                var pages = GetFlooredInt(totalProducts, request.Limit.Value);
                totalPagesWithCurrentLimit = pages < 1 ? 1 : pages;
            }

            var productsResponse = new ProductsResponse()
            {
                Products = MapProductsToResponse(products),
                TotalPagesWithCurrentLimit = totalPagesWithCurrentLimit
            };

            _logger.LogInformation($"{DateTime.Now} | End getting all products");

            return Ok(productsResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [Route("by-user")]
    public IActionResult GetAllUserProducts([FromQuery] ProductsRequest request)
    {
        try
        {
            _logger.LogInformation($"{DateTime.Now} | Start getting all products");
            var products = _productService.GetUserProducts(request, User.Identity.Name);
            var totalProducts = _productService.GetUserProductsCount(request.SearchValue, User.Identity.Name);
            var totalPagesWithCurrentLimit = int.MaxValue;

            if (request.Limit.HasValue && request.Limit > 0)
            {
                var pages = GetFlooredInt(totalProducts, request.Limit.Value);
                totalPagesWithCurrentLimit = pages < 1 ? 1 : pages;
            }

            var productsResponse = new ProductsUserResponse()
            {
                Products = MapUserProductsToResponse(products),
                TotalPagesWithCurrentLimit = totalPagesWithCurrentLimit
            };

            _logger.LogInformation($"{DateTime.Now} | End getting all products");

            return Ok(productsResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [Route("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult AddNewProduct([FromBody] ProductModel model)
    {
        try
        {
            _logger.LogInformation($"{DateTime.Now} | Start adding new product");
            _productService.AddNewProduct(new Product() { Name = model.Name, Calories = model.Calories});
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
    [Route("admin/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateProduct(int id, [FromBody] ProductModel model)
    {
        try
        {
            _logger.LogInformation($"{DateTime.Now} | Start adding new category");
            model.Id = id;
            _productService.Update(model);
            _logger.LogInformation($"{DateTime.Now} | End adding new category");

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    [Route("")]
    public IActionResult AddUserProduct([FromBody] ProductUserModel model)
    {
        try
        {
            _logger.LogInformation($"{DateTime.Now} | Start adding new product to user");
            _productService.AddUserProduct(model.ProductId, model.Amount, model.Type, User.Identity.Name);
            _logger.LogInformation($"{DateTime.Now} | End adding new category to user");

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
    public IActionResult UpdateUserProduct(int id, [FromBody] ProductUserModel model)
    {
        try
        {
            _logger.LogInformation($"{DateTime.Now} | Start adding new category");
            model.ProductId = id;
            _productService.UpdateUserProduct(model.ProductId, model.Amount, model.Type, User.Identity.Name);
            _logger.LogInformation($"{DateTime.Now} | End adding new category");

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    private IEnumerable<AllProducts> MapProductsToResponse(IEnumerable<Product> products)
    {
        var response = products.Select(p => new AllProducts()
        {
            ProductId = p.Id,
            Calories = p.Calories,
            Name = p.Name
        });

        return response;
    }

    private IEnumerable<AllUserProducts> MapUserProductsToResponse(IEnumerable<Product> products)
    {
        var response = products.Select(p => new AllUserProducts()
        {
            ProductId = p.Id,
            Calories = p.Calories,
            Amount = p.ProductUsers?.First().Amount,
            Name = p.Name
        });

        return response;
    }

    private int GetFlooredInt(int a, int b) => (a + b - 1) / b;
}