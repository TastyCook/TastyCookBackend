using MassTransit;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Services;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.RecipesAPI.Consumers;

public class ProductCreatedConsumer : IConsumer<ProductItemCreated>
{
    private readonly ProductService _productService;
    private readonly ILogger<ProductCreatedConsumer> _logger;

    public ProductCreatedConsumer(ProductService productService, ILogger<ProductCreatedConsumer> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductItemCreated> context)
    {
        var message = context.Message;
        _logger.LogInformation($"{DateTime.Now} | Consumed: {context.Message}");
        var item = _productService.GetById(message.Id);

        if (item is not null)
        {
            return;
        }

        _productService.AddNewProduct(new Product
        {
            Id = message.Id,
            Calories = message.Calories,
            Localization = (Localization)message.Localization,
            Name = message.Name
        });
        _logger.LogInformation($"{DateTime.Now} | Product added after consume: {context.Message}");
    }
}