using MassTransit;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Services;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.RecipesAPI.Consumers;

public class UserCreatedConsumer : IConsumer<UserItemCreated>
{
    private readonly UserService _userService;
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(UserService userService, ILogger<UserCreatedConsumer> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserItemCreated> context)
    {
        var message = context.Message;
        _logger.LogInformation($"{DateTime.Now} | Consumed: {context.Message}");
        var item = _userService.GetById(message.Id);

        if (item is not null)
        {
            return;
        }

        _userService.Add(new User { Id = message.Id, Email = message.Email/*, Password = message.Password*/, UserName = message.Username, Role = message.Role});
        _logger.LogInformation($"{DateTime.Now} | User added after consume: {context.Message}");
    }
}