using MassTransit;
using TastyCook.RecipesAPI.Services;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.RecipesAPI.Consumers;

public class ProductDeletedConsumer : IConsumer<ProductItemDeleted>
{
    private readonly ProductService _productService;
    private readonly ILogger<ProductDeletedConsumer> _logger;

    public ProductDeletedConsumer(ProductService productService, ILogger<ProductDeletedConsumer> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductItemDeleted> context)
    {
        var message = context.Message;
        _logger.LogInformation($"{DateTime.Now} | Consumed: {context.Message}");

        var item = _productService.GetById(message.Id);
        if (item is null)
        {
            return;
        }

        _productService.DeleteById(message.Id);
        _logger.LogInformation($"{DateTime.Now} | Product deleted after consume: {context.Message}");
    }
}