using MassTransit;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Services;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.RecipesAPI.Consumers;

public class ProductUpdatedConsumer : IConsumer<ProductItemUpdated>
{
    private readonly ProductService _productService;
    private readonly ILogger<ProductUpdatedConsumer> _logger;

    public ProductUpdatedConsumer(ProductService productService, ILogger<ProductUpdatedConsumer> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductItemUpdated> context)
    {
        var message = context.Message;
        _logger.LogInformation($"{DateTime.Now} | Consumed: {context.Message}");

        var item = _productService.GetById(message.Id);
        if (item is null)
        {
            _productService.AddNewProduct(new Product
            {
                Id = message.Id,
                Calories = message.Calories,
                Localization = (Localization)message.Localization,
                Name = message.Name
            });
            _logger.LogInformation($"{DateTime.Now} | Product added during update after consume: {context.Message}");
        }

        _productService.Update(new Product
        {
            Id = message.Id,
            Calories = message.Calories,
            Localization = (Localization)message.Localization,
            Name = message.Name
        });
        _logger.LogInformation($"{DateTime.Now} | Product updated after consume: {context.Message}");
    }
}