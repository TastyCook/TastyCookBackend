using MassTransit;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Services;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.RecipesAPI.Consumers;

public class ProductUserCreatedConsumer : IConsumer<ProductUserItemCreated>
{
    private readonly ProductService _productService;
    private readonly ILogger<ProductUserCreatedConsumer> _logger;

    public ProductUserCreatedConsumer(ProductService productService, ILogger<ProductUserCreatedConsumer> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductUserItemCreated> context)
    {
        var message = context.Message;
        _logger.LogInformation($"{DateTime.Now} | Consumed: {context.Message}");
        var item = _productService.GetProductUserByIds(message.ProductId, message.UserId);

        if (item is not null)
        {
            return;
        }

        _productService.AddUserProduct(new ProductUser
        {
            ProductId = message.ProductId,
            UserId = message.UserId,
            Amount = message.Amount
        });
        _logger.LogInformation($"{DateTime.Now} | ProductUser added after consume: {context.Message}");
    }
}