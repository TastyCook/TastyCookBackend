using MassTransit;
using TastyCook.RecipesAPI.Models;
using TastyCook.RecipesAPI.Services;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.RecipesAPI.Consumers;

public class UserDeletedConsumer : IConsumer<UserItemDeleted>
{
    private readonly UserService _userService;
    private readonly ILogger<UserDeletedConsumer> _logger;

    public UserDeletedConsumer(UserService userService, ILogger<UserDeletedConsumer> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserItemDeleted> context)
    {
        var message = context.Message;
        _logger.LogInformation($"{DateTime.Now} | Consumed: {context.Message}");

        var item = _userService.GetById(message.Id);
        if (item is null)
        {
            return;
        }

        _userService.DeleteById(message.Id);
        _logger.LogInformation($"{DateTime.Now} | User deleted after consume: {context.Message}");
    }
}