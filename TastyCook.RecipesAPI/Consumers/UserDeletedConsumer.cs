using MassTransit;
using TastyCook.RecipesAPI.Models;
using TastyCook.RecipesAPI.Services;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.RecipesAPI.Consumers;

public class UserDeletedConsumer : IConsumer<UserItemDeleted>
{
    private readonly UserService _userService;

    public UserDeletedConsumer(UserService userService)
    {
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<UserItemDeleted> context)
    {
        var message = context.Message;

        var item = _userService.GetById(message.Id);
        if (item is null)
        {
            return;
        }

        _userService.DeleteById(message.Id);
    }
}