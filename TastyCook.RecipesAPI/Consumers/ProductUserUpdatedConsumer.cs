using MassTransit;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Services;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.RecipesAPI.Consumers;

public class ProductUserUpdatedConsumer : IConsumer<ProductUserItemUpdated>
{
    private readonly ProductService _productService;
    private readonly ILogger<ProductUserUpdatedConsumer> _logger;

    public ProductUserUpdatedConsumer(ProductService productService, ILogger<ProductUserUpdatedConsumer> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductUserItemUpdated> context)
    {
        var message = context.Message;
        _logger.LogInformation($"{DateTime.Now} | Consumed: {context.Message}");

        var item = _productService.GetProductUserByIds(message.ProductId, message.UserId);
        if (item is null)
        {
            _productService.AddUserProduct(new ProductUser
            {
                ProductId = message.ProductId,
                UserId = message.UserId,
                Amount = message.Amount
            });
            _logger.LogInformation($"{DateTime.Now} | ProductUser added during update after consume: {context.Message}");
        }

        _productService.UpdateUserProduct(new ProductUser
        {
            ProductId = message.ProductId,
            UserId = message.UserId,
            Amount = message.Amount
        });
        _logger.LogInformation($"{DateTime.Now} | ProductUser updated after consume: {context.Message}");
    }
}