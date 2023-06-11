using MassTransit;
using TastyCook.ProductsAPI.Entities;
using TastyCook.ProductsAPI.Services;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.ProductsAPI.Consumers;

public class UserUpdatedConsumer : IConsumer<UserItemUpdated>
{
    private readonly UserService _userService;
    private readonly ILogger<UserItemUpdated> _logger;

    public UserUpdatedConsumer(UserService userService, ILogger<UserItemUpdated> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserItemUpdated> context)
    {
        var message = context.Message;
        _logger.LogInformation($"{DateTime.Now} | Consumed: {context.Message}");

        var item = _userService.GetById(message.Id);
        if (item is null)
        {
            _userService.Add(new User { Id = message.Id, Email = message.Email, UserName = message.Username, Role = message.Role });
            _logger.LogInformation($"{DateTime.Now} | User added during update after consume: {context.Message}");
        }

        _userService.Update(new User { Id = message.Id, Email = message.Email, UserName = message.Username, Role = message.Role });
        _logger.LogInformation($"{DateTime.Now} | User updated after consume: {context.Message}");
    }
}