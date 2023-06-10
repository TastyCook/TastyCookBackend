using MassTransit;
using TastyCook.ProductsAPI.Entities;
using TastyCook.ProductsAPI.Services;

namespace TastyCook.ProductsAPI.Consumers;

public class UserCreatedConsumer : IConsumer<Contracts.Contracts.UserItemCreated>
{
    private readonly UserService _userService;
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(UserService userService, ILogger<UserCreatedConsumer> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<Contracts.Contracts.UserItemCreated> context)
    {
        var message = context.Message;
        _logger.LogInformation($"{DateTime.Now} | Consumed: {context.Message}");
        var item = _userService.GetById(message.Id);

        if (item is not null)
        {
            return;
        }

        _userService.Add(new User { Id = message.Id, Email = message.Email, UserName = message.Username });
        _logger.LogInformation($"{DateTime.Now} | User added after consume: {context.Message}");
    }
}