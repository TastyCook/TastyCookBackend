using MassTransit;
using TastyCook.RecipesAPI.Services;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.RecipesAPI.Consumers;

public class ProductUserDeletedConsumer : IConsumer<ProductUserItemDeleted>
{
    private readonly ProductService _productService;
    private readonly ILogger<ProductUserDeletedConsumer> _logger;

    public ProductUserDeletedConsumer(ProductService productService, ILogger<ProductUserDeletedConsumer> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductUserItemDeleted> context)
    {
        var message = context.Message;
        _logger.LogInformation($"{DateTime.Now} | Consumed: {context.Message}");

        var item = _productService.GetProductUserByIds(message.ProductId, message.UserId);
        if (item is null)
        {
            return;
        }

        _productService.DeleteUserProductsById(message.ProductId, message.UserId);
        _logger.LogInformation($"{DateTime.Now} | ProductUser deleted after consume: {context.Message}");
    }
}