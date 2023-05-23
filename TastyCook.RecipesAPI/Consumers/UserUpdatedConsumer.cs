using MassTransit;
using TastyCook.RecipesAPI.Entities;
using TastyCook.RecipesAPI.Models;
using static TastyCook.Contracts.Contracts;

namespace TastyCook.RecipesAPI.Consumers;

public class UserUpdatedConsumer : IConsumer<UserItemUpdated>
{
    private readonly UserService _userService;

    public UserUpdatedConsumer(UserService userService)
    {
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<UserItemUpdated> context)
    {
        var message = context.Message;

        var item = _userService.GetById(message.Id);
        if (item is null)
        {
            _userService.Add(new User { Id = message.Id, Email = message.Email, Password = message.Password, UserName = message.Username });
        }

        _userService.Update(new User { Id = message.Id, Email = message.Email, Password = message.Password, UserName = message.Username});
    }
}